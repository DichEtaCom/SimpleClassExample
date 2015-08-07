using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Forms;
using ec.WPF.Localization;
using feni01SQL;
using Microsoft.Office.Interop.Word;
using Org.BouncyCastle.Crypto.Generators;
using org.pdfclown.documents;
using org.pdfclown.documents.contents.composition;
using org.pdfclown.files;
using xuFont = org.pdfclown.documents.contents.fonts;


using System;
using System.Collections.Generic;
using nativeIo = System.IO;
using Form = org.pdfclown.documents.interaction.forms.Form;
using MessageBox = System.Windows.Forms.MessageBox;
using NLog;
using ec.WPF.Localization;
using DataTable = System.Data.DataTable;
using Field = org.pdfclown.documents.interaction.forms.Field;


namespace FENI2011.Bewerber
{
    public class BewerberReadFromPdf
    {
 	private static readonly Logger Log = LogManager.GetCurrentClassLogger();
    private static int keIdValRecursionCount = 0;

        public static void ExtractContentFromPdf(string filepath,string vornameFromBMaske, string nameFromBMaske,string niId, string bemerkungContent,long bwbrID )
        {
            var returnedArray = FormPdfRead(filepath);

            var fullNameFromPdf = returnedArray[0].ToString();
            var kenntnisseNiveau = (Dictionary<string, object>)returnedArray[1];
            var versions = (Dictionary<string, object>)returnedArray[2];

            if (kenntnisseNiveau.Count == 0 && versions.Count == 0)
            {
                return;
            }

            var kennNivArr = ConvertDictToArray(kenntnisseNiveau, kenntnisseNiveau.Count);
            var versionAray = ConvertDictToArray(versions, versions.Count);
           

            string conString = clsGlobalConst.connStr();
            try
            {
                        //feni01SQL.cls_GlobalConst.DebugMode = true;
                        string fNamePdf = fullNameFromPdf.Replace(" ", "").ToLower();
                        vornameFromBMaske = vornameFromBMaske.Replace(" ", "").ToLower();
                        nameFromBMaske = nameFromBMaske.Replace(" ", "").ToLower();
                        if (fNamePdf.Equals((vornameFromBMaske + nameFromBMaske)) || fNamePdf.Equals(nameFromBMaske + vornameFromBMaske) || fNamePdf.Equals(vornameFromBMaske + "," + nameFromBMaske) || fNamePdf.Equals(nameFromBMaske+"," + vornameFromBMaske))
                        {
                            var dataTable = DataTableBewerb(conString, vornameFromBMaske, nameFromBMaske, bwbrID);

                            //var bewerberId = BewerberIdReturn(conString, vornameFromBMaske, nameFromBMaske);
                             var bewerberId = bwbrID.ToString();

                            // index of Array from Dictionary
                            var notFoundValList = new List<string>();

                            //kennNivArr = BadvalReplace(kennNivArr);
                            //versionAray = BadvalReplace(versionAray);

                            //Compare content of Datatable, which extract original content of Bewerber from Feni_MD and compare with
                            //the content of the Array(values from PDF), if Kenntnisse is new then insert, 
                            //if exist, but Niveau or Version is different, then update old values with new from PDF file
                            for (int j = 0; j < kennNivArr.Length / 2; j++)
                            {
                              string s = SearchDupInsUpd(conString,j,kennNivArr,versionAray,dataTable,niId,bewerberId,notFoundValList,vornameFromBMaske,nameFromBMaske);
                                if (s.Length > 0)
                                {
                                    notFoundValList.Add(s);
                                }
                            }

                            #region AddNotFoundKenntnisseInBemerkungField
                            if (notFoundValList.Count>0 )//notFoundKenntCount > 0)
                            {
                               string bemerkungValue = BemerkungValueCalc(bewerberId, bemerkungContent, notFoundValList);
                                BemerkungUpdate(conString, bewerberId, bemerkungValue);
                            }
                            #endregion
                        }
                        else if (fullNameFromPdf.Trim().Length==0)
                        {
                             MessageBox.Show(LanguageDictionary.Current.Translate<string>("Bewerber.ImportAusSkillsPDFEmptyName.Text", "Text",
                                 "You drag PDF file with an empty name. Please enter the name or drag another PDF file.",
                                  "Pdf Import", MessageBoxButtons.OK, MessageBoxIcon.Information));
                        }
                        else
                        {//ImortAusSkillsPDF
                            feni01SQL.cls_GlobalConst.DebugMode = true;
                            MessageBox.Show(LanguageDictionary.Current.Translate<string>("Bewerber.ImportAusSkillsPDFWrongName.Text", "Text",
                                 "Der Skillplan konnte nicht importiert werden, weil der {0} nicht übereinstimmt. Bitte ändern Sie den Namen des Bewerbers im Skillplan wie folgt: {1} {2}." +
                                 "Danach ziehen Sie den Skillplan nochmals auf den Reiter Kenntnisse des Bewerbers, um die Skills zu importieren.", fullNameFromPdf, vornameFromBMaske, nameFromBMaske),
                                  "Pdf Import", MessageBoxButtons.OK,MessageBoxIcon.Information);
                        }

                
            }
            catch (Exception ex)
            {
                Log.ErrorException("Exception in BewerberReadFromPdf\\ExtractContentFromPdf", ex);
            }
        }

        public static DataTable DataTableBewerb(string conString, string vornameFromBMaske, string nameFromBMaske, long brwId)
        {
            using (var connection = new SqlConnection(conString))
            {
                connection.Open();
                using (var dt = new DataTable())
                {
                    var dataAdapt = new SqlDataAdapter(
                     "SELECT [BW_BEWERBER].b_Vname, [BW_BEWERBER].b_Name ,[ke_ref_id] ,ke_name , ke_ref_ke_id ,[ke_status],ke_einschaetzung, ke_Bemerkung, [ke_ref_a_id] FROM [BW_Kenntnisse] join [BW_BEWERBER] on [BW_Kenntnisse].ke_ref_id = [BW_BEWERBER].b_id join [SYS_KE" +
                     clsGlobalConst.cSysLangTblSuffix + "] on [BW_Kenntnisse].ke_ref_ke_id = [SYS_KE" + clsGlobalConst.cSysLangTblSuffix + "].ke_id where b_Vname like'" + vornameFromBMaske + "%' and b_Name like'%" +
                     nameFromBMaske + "%' and ke_status='b' and ke_ref_id = " + brwId, connection);
                    dataAdapt.Fill(dt);
                    return dt;
                }
            }
        }



        public static string SearchDupInsUpd(string conString, int j, object[] kennNivArr, object[] versionAray, DataTable dt, string niId, string bewerberId, List<string> notFoundValList, string vName, string name)
        {
                int duplInd = 0; //Index of duplicate in the array for future update if needed
                string kenntPdf = kennNivArr[j * 2].ToString();
                string niveauPdf = kennNivArr[j * 2 + 1].ToString();
                int keIdVal = KeIdValReturn(conString, kenntPdf);
                int checkForDuplicate = 0;

              
                for (int k = 0; k < dt.Rows.Count; k++) //k is index for Datatable
                {
                    int cht = Convert.ToInt16(dt.Rows[k][4]);
                    if (kenntPdf == dt.Rows[k][3].ToString() || (keIdVal == Convert.ToInt16(dt.Rows[k][4]) && keIdVal!=0))
                    {
                        checkForDuplicate++;
                        duplInd = k;
                    }
                }

                #region Insert
                if (checkForDuplicate == 0)
                {
                    string returnIns = InsertSql(conString, niId, dt, bewerberId, kenntPdf, niveauPdf, versionAray, vName,name, keIdVal);
                    return returnIns;
                }
                #endregion

                #region Update
                bool updateOrNot = (checkForDuplicate != 0) && (DeleteSpecCharAndLowerString(dt.Rows[duplInd][3].ToString()) == DeleteSpecCharAndLowerString(kenntPdf)) && (Convert.ToInt16(dt.Rows[duplInd][6]) != Convert.ToInt16(kennNivArr[Array.IndexOf(kennNivArr, kenntPdf) + 1]) ||
                                    dt.Rows[duplInd][7].ToString() != (dt.Rows[duplInd][7].ToString().Length > 0 ? versionAray[Array.IndexOf(versionAray, kenntPdf) + 1].ToString() : dt.Rows[duplInd][7].ToString()));
                if (updateOrNot)
                {
                    string returno = UpdateSql(conString, kenntPdf, niveauPdf, versionAray, kennNivArr, dt, duplInd);
                    if (returno.Length > 4)
                    {
                        notFoundValList.Add(returno);
                        return returno;
                    }
                }
                #endregion

                return "";
        }

        public static object[] ConvertDictToArray(Dictionary<string, object> dictionary, int dictionaryCount)
        {
            var array = new object[dictionaryCount * 2];
            int index = 0;
            for (int j = dictionaryCount; j > 0; j--)
            {
                array[index] = dictionary.Keys.ElementAt(dictionary.Count - j);
                index++;
                array[index] = dictionary.Values.ElementAt(dictionary.Count - j);
                index++;
            }
            return array;
        }

        public static string BadvalReplace(string kenntPdf)
        {
            #region BadOrWrongValuesFromPDF
            var badValues = new[]
                    {
                         //AVIATION Skillplan Technik and Tools File(some kenntnisse may be the same in this and plantIng Skillplan file)
                         //For these Kenntnisse there are no exact match in the SQL database
                         "PlanungProjektierung", "Planung/Projektierung","Test Validierung", "Test, Validierung", "VPM  VPLM", "VPM / VPLM ",  //Analyse, Treiber, "Windows Server" ,

                         //AVIATION Skillplan Technik
                         "FEMBerechnung", "FEM-Berechnung", "User Help Desk Level 1", "User Help Desk Level1", "User Help Desk Level 2", "User Help Desk Level2", "Mech Systeme" , "Mechanische. Systeme", 
                         "LuftahrzeugPrüferlizenz Klasse", "Luftfahrzeug-Prüferlizenz-Klasse", "MontageFügetechnik", "Montage-/ Fügetechnik", "HFTechnik", "HF-Technik", "NCProgrammierung", "NC-Programmierung", 
                         "BusSysteme", "Bus-Systeme", "Prozessvisualisierung", "Prozess-Visualisierung", "Routing Layouterstellung", "Routing, Layouterstellung", "Fehleranalyse behebung", "Fehleranalyse/ -behebung",
                          "RealTimeAnwendungen", "Real-Time-Anwendungen", "StandAloneSysteme", "Stand-Alone-Systeme","ATAKapitel", "ATA-Kapitel",  "ClientServerApplikationen", "Client-/Server-Applikationen",
                         "Visualisierung GUI", "Visualisierung, GUI", "WebApplikationen", "Web-Applikationen",
                         
                         //AVIATION Skillplan Tools
                         "AutoCADMechPP", "AutoCAD/MechPP", "Ideas", "I-deas", "ProENGINEER", "Pro/ENGINEER", "VPNLInk", "VPN", "DATig", "DA Tig", "ASCETSD", "ASCET-SD", "OOAOOD", "OOA/OOD",
                         "MSSQL Server", "MS SQL Server", "Windows NT  2000", "Windows NT / 2000", "Firewalls", "Firewall", "MS  Exchange Server", "MS Exchange Server", "mySAPPLM", "mySAP/PLM",
                         "PROINTRALINK", "Pro/INTRALINK", "MSOffice", "MS Office", "MSProject", "MS Project",


                         //plantIng Skillplan FILE
                         //Energie,Planung Mitarbeiterführung (there is just 1st or 2nd word), Rohrleitungsspezifikationen (there is Rohrleitungsspezifikation), Kabelberechnungen (there is Kabelberechnung), ProEngineer (Pro/ENGINEER), ProStructure (ProStructures),
                         // 2D Planung and 3D Planung (NOT), Umschlusskonzepte (NOT), Koordination BGV A1 (NOT), Qualitätsprüfung (NOT), VAwS (Not), DCS Systeme (NOT), Eigensicherheitsnachweis (NOT), Conval(NOT), FERO (NOT), PlantSpace Isometrics(NOT), Roser(NOT), SmartPlant PID (NOT)
                         "CADPlanung", "CAD-Planung", "FEPipe", "FE/Pipe", "Nozzlepro","Nozzle/pro",

                         //page 1
                        "Öl  Gas", "Öl- und Gas", "BeratungConsulting", "Beratung/Consulting", "Aufwandschätzung", "Aufwandsschätzung","LastenPflichtenhefte", "Lasten-/Pflichtenheft", "Machbarkeitsstudie", "Machbarkeitsstudien",
                        "HAZOPPAAG Verfahren", "HAZOP/PAAG-Verfahren", "ProzesssSimulation", "Prozess-Simulation", "RI Schemata", "R + I Schemata", "Aufmaß  Aufnahme", "Aufmaß/Aufnahme", "ENDINISO", "EN/DIN/ISO",   "ASMEANSI", "ASME/ANSI",
                        //page 2
                        "Ausschreibung  Vergabe", "Ausschreibung/Vergabe", "ExSchutz", "Ex-Schutz", "HOAI Phasen", "HOAI-Phasen", "PipeStressberechnung", "Pipe-Stressberechnung", "ESDSysteme", "ESD Systeme", "EX Geräte", "Ex-Geräte",
                         "VDE100", "VDE 100", "VDE113", "VDE 113", "ProzessAutomatisierung", "Prozess-Automatisierung", "ProzessAnalysen", "Prozess-Analysen", "PlantSpace P  ID", "PlantSpace P&ID",  "RStab", "R-Stab", "SAPAnwender", "SAP-Anwender",


                         //Other Files With New Kenntnisse (firstly Kenntnisse that have no records in DB, then others)
                         "Versuch Inbetriebnahme","Versuch, Inbetriebnahme", "Terminplanungverfolgung", "Terminplanung/-verfolgung", "Lieferantenauswahlaudits", "Lieferantenauswahl/-audits", //Windows Server, ENOVIA PLM
                         "Anleitungen Handbücher", "Anleitungen, Handbücher",

                         "KälteKlimaanlagen", "Kälte-/Klimaanlagen", "ControllingReporting", "Controlling/Reporting", "Messen Prüfen Befunden", "Messen, Prüfen, Befunden", "SupplyChainManagement", "Supply-Chain-Management",
                         "AllenBradley", "Allen-Bradley", "BoschRexroth", "Bosch-Rexroth", "NET", ".NET","MSSQLServer", "MS SQL Server","UDBDB2", "UDB/DB2", "MSExchange Server", "MS Exchange Server", "ProINTRALINK", "Pro/INTRALINK",
                         "SAP Modul angeben", "SAP-Module angeben", "8DReport", "8D-Report"

                    };
            #endregion


            if (Array.IndexOf(badValues, kenntPdf) != -1) kenntPdf = badValues[Array.IndexOf(badValues, kenntPdf) + 1];
            
            return kenntPdf;
        }

        public static string InsertSql(string conString, string niId, DataTable dt, string bewerberId, string kenntPdf, string niveauPdf, object[] versionAray, string vName,string name, int keIdVal)
        {
            using (var connection = new SqlConnection(conString))
            {
                connection.Open();
               
                if (dt.Rows.Count==0)
                {
                    dt.Rows.Add(vName, name, bewerberId, kenntPdf, keIdVal, "B",niveauPdf);
                }
                else
                {
                    dt.Rows.Add(dt.Rows[0][0], dt.Rows[0][1], bewerberId, kenntPdf, keIdVal,
                   dt.Rows[0][5], dt.Rows[0][6]);
                }

               
                string query =
                    "INSERT INTO BW_Kenntnisse (ke_ref_ke_id,ke_ref_id,ke_einschaetzung, ke_status,OfficeID,ni_id,ke_Bemerkung)";
                query += "VALUES (@ke_ref_ke_id, @ke_ref_id,@ke_einschaetzung, @ke_status, @OfficeID,@ni_id,@ke_Bemerkung)";
                var insertCmd = new SqlCommand(query, connection);

                if (keIdVal != 0)
                {
                    insertCmd.Parameters.AddWithValue("@ke_ref_ke_id", keIdVal);
                    insertCmd.Parameters.AddWithValue("@ke_ref_id", bewerberId);
                    insertCmd.Parameters.AddWithValue("@ke_einschaetzung", ReplaceBadNiveau(niveauPdf));
                    insertCmd.Parameters.AddWithValue("@ke_status", "B");
                    insertCmd.Parameters.AddWithValue("@OfficeID", 999);
                    insertCmd.Parameters.AddWithValue("@ni_id", niId);

                    var versia = "";
                    for (int l = 0; l < versionAray.Length / 2; l++)
                    {
                        if (versionAray[l * 2].ToString() == kenntPdf)
                        {
                            versia = versionAray[l * 2 + 1].ToString();
                        }
                    }
                    insertCmd.Parameters.AddWithValue("@ke_Bemerkung",
                        versia.ToLower() == "autocad" ? "" : versia);
                    insertCmd.ExecuteNonQuery();
                }
                else
                {
                    return BadvalReplace(kenntPdf) + " (" + niveauPdf + ")";
                }
                return "";
            }
        }

        public static string UpdateSql(string conString, string kenntPdf, string niveauPdf, object[] versionAray, object[] kennNivArr, DataTable dt, int duplInd)
        {
            using (var connection = new SqlConnection(conString))
            {
                connection.Open();
                int? keIdVal = Convert.ToInt16(dt.Rows[duplInd][4]);
                string query =
                    "Update BW_Kenntnisse  SET ke_einschaetzung=@ke_einschaetzung,ke_Bemerkung=@ke_Bemerkung where [BW_Kenntnisse].[ke_ref_ke_id] = " +
                    keIdVal;
                var updateCommand = new SqlCommand(query, connection);

                if (keIdVal != 0)
                {
                    int versionInd = Array.IndexOf(versionAray, kenntPdf) + 1;
                    string einshso = kennNivArr[Array.IndexOf(kennNivArr, kenntPdf) + 1].ToString();
                    updateCommand.Parameters.AddWithValue("@ke_einschaetzung",
                       ReplaceBadNiveau(kennNivArr[Array.IndexOf(kennNivArr, kenntPdf) + 1].ToString()));
                    updateCommand.Parameters.AddWithValue("@ke_Bemerkung",
                        versionInd == 0 ? "" : versionAray[versionInd].ToString());
                    updateCommand.ExecuteNonQuery();
                }
                else
                {
                    return BadvalReplace(kenntPdf) + " (" + niveauPdf + ")";
                    //Console.WriteLine("Wrong nameUPDATE: {0}", kenntPdf);
                }
                return "";
            }
        }

      

        public static int KeIdValReturn(string conString, string kenntPdf)
        {
            using (var connection = new SqlConnection(conString))
            {
                connection.Open();
                var updEinschaetzung =
                          new SqlCommand( "SELECT distinct [SYS_KE"+ clsGlobalConst.cSysLangTblSuffix +"].[ke_id] FROM [SYS_KE" +
                              clsGlobalConst.cSysLangTblSuffix +
                              "] left join [BW_Kenntnisse] on [SYS_KE" +
                              clsGlobalConst.cSysLangTblSuffix +
                              "].ke_id=[BW_Kenntnisse].[ke_ref_ke_id] where LOWER(replace(replace(replace(replace(replace(replace(replace(ke_Name,'''',''),' ',''),'-',''),'/',''),',',''),'.',''),'&',''))= '" +
                              DeleteSpecCharAndLowerString(kenntPdf) + "'", connection);
                int? keIdVal = Convert.ToInt16(updEinschaetzung.ExecuteScalar());
                if (keIdVal == 0 && keIdValRecursionCount==0)
                {
                    keIdValRecursionCount++;
                    keIdVal = KeIdValReturn(conString, BadvalReplace(kenntPdf));
                }
                keIdValRecursionCount = 0;
                return Convert.ToInt16(keIdVal);
            }
         }

        public static string DeleteSpecCharAndLowerString(string input)
        {
            input = input.Replace(" ", "").Replace(",", "").Replace(".", "").Replace("-", "").Replace("'", "").Replace("&", "").Replace("/", "").ToLower();
            return input;
        }


        public static string BewerberIdReturn(string conString, string vornameFromBMaske, string nameFromBMaske)
        {
            using (var connection = new SqlConnection(conString))
            {
                connection.Open();
                var cmd =
                             new SqlCommand(
                                 "SELECT [BW_BEWERBER].b_id from BW_BEWERBER where b_Vname='" + vornameFromBMaske +
                                 "' and b_Name= '" + nameFromBMaske + "'", connection);
                //old query
                //new SqlCommand(
                //    "SELECT [ke_ref_id] FROM [BW_Kenntnisse] join [BW_BEWERBER] on [BW_Kenntnisse].ke_ref_id = [BW_BEWERBER].b_id join [SYS_KE" +
                //    clsGlobalConst.cSysLangTblSuffix + "]  on [BW_Kenntnisse].ke_ref_ke_id = [SYS_KE" + clsGlobalConst.cSysLangTblSuffix + "].ke_id where b_Vname='" + vornameFromBMaske +
                //    "' and b_Name= '" + nameFromBMaske + "'", connection);
                var bewerberId = cmd.ExecuteScalar().ToString();
                return bewerberId;
            }
        }

        public static string BemerkungValueCalc(string bewerberId, string bemerkungContent, List<string> notFoundValList)
        {
                string bemerkungValue = bemerkungContent;

                try
                {
                    string bemerkungPdfNotFound = "";
                    var re = new Regex(@"\(\d+\)");

                    if (bewerberId.Length > 0)
                    {

                        bemerkungContent = bemerkungContent.Replace("Kenntnisse:/", "KenntnisseFromPDF:");
                        if (bemerkungContent.Length >= 0)
                        {
                            if (bemerkungContent.Contains("KenntnisseFromPDF"))
                            {
                                bemerkungContent = bemerkungContent.Substring(0, bemerkungContent.IndexOf("KenntnisseFromPDF: "));
                            }

                            for (int i = 0; i < notFoundValList.Count; i++)
                            {
                                Match mPdf = re.Match(notFoundValList[i]);
                                string kenntniseWoNiveau = notFoundValList[i].Remove(mPdf.Index - 1);
                                if (bemerkungContent.Contains(kenntniseWoNiveau))
                                {
                                    if (re.IsMatch(bemerkungContent.Substring(bemerkungContent.IndexOf(kenntniseWoNiveau) +
                                                kenntniseWoNiveau.Length, 4)))
                                    {
                                        Match mBemerkFeni = re.Match(bemerkungContent.Substring(bemerkungContent.IndexOf(kenntniseWoNiveau) +
                                                    kenntniseWoNiveau.Length, 4));
                                        if (mPdf.Value == mBemerkFeni.Value)
                                        {
                                            //Do nothing
                                        }
                                        else
                                        {
                                            bemerkungContent =
                                                bemerkungContent.Replace(kenntniseWoNiveau + mBemerkFeni.Value,
                                                    kenntniseWoNiveau + mPdf.Value);
                                            bemerkungContent =
                                                bemerkungContent.Replace(kenntniseWoNiveau + " " + mBemerkFeni.Value,
                                                    kenntniseWoNiveau + " " + mPdf.Value);
                                        }
                                    }
                                    else
                                    {
                                        bemerkungContent = bemerkungContent.Replace(kenntniseWoNiveau, notFoundValList[i]);
                                    }
                                }
                                else
                                {
                                    bemerkungPdfNotFound += notFoundValList[i] + "\n";
                                }
                            }
                            if (bemerkungContent.Contains("KenntnisseFromPDF:") &&
                                bemerkungPdfNotFound.Length > 0)
                            {
                                bemerkungValue =
                                    bemerkungContent.Substring(0, bemerkungContent.IndexOf("KenntnisseFromPDF: ")) +
                                    "\nKenntnisseFromPDF: " + bemerkungPdfNotFound;
                            }
                            else if (bemerkungPdfNotFound.Length > 0)
                            {
                                bemerkungValue = bemerkungContent + "\nKenntnisseFromPDF: " + bemerkungPdfNotFound;
                            }
                            else
                            {
                                bemerkungValue = bemerkungContent;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.ErrorException("Exception in BewerberReadFromPDF\\BemerkungValueCalc", ex);
                }
                return bemerkungValue;
        }

        public static void BemerkungUpdate(string conString, string bewerberId, string bemerkungValue)
        {
            try
            {
                using (var connection = new SqlConnection(conString))
                {
                    connection.Open();
                    var updateBemerkungCommand =
                        new SqlCommand("Update BW_BEWERBER set b_Bemerkung=@b_Bemerkung where b_id=" + bewerberId,
                            connection);
                    updateBemerkungCommand.Parameters.AddWithValue("@b_Bemerkung", bemerkungValue);
                    updateBemerkungCommand.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Log.ErrorException("Exception in BewerberReadFromPDF\\BemerkungUpdate", ex);
            }
        }

        #region ReadFromPdfFormsMethod
        public static object[] FormPdfRead(string filePath)
        {
            var kenntNiveauVersVorname = new object[3];

            try
            {
                var kenntnisseNiveau = new Dictionary<string, object>();
                var tempWeitereKennt = new Dictionary<string, object>();
                var dictVersions = new Dictionary<string, object>();
                string vornameUndName = "";
                int counterForOldFromat = 0;


                // Opening the PDF file
                if (filePath.ToLower().Contains(".pdf"))
                {
                    using (var file = new File(filePath))
                    {
                        var document = file.Document;

                        //Get the PdfForms!
                        var form = document.Form;
                        if (!form.Exists()) { } // { Console.WriteLine("\nNo PdfForms available"); }
                        else
                        {
                            //Console.WriteLine("\nIterating through the fields collection...\n");

                            //Showing the PdfForms fields...
                            var objCounters = new Dictionary<string, int>();
                            foreach (Field field in form.Fields.Values)
                            {
                                string typeName = field.GetType().Name;

                                if (field.Value != null && field.Value.ToString().Length > 0)
                                {
                                    if (field.FullName.Contains("Name") || field.FullName.Contains("Vorname"))
                                    {
                                        vornameUndName = field.Value.ToString();
                                    }

                                    #region ExtractVersion

                                    if (field.FullName.Contains("Version"))
                                    {
                                        dictVersions.Add(field.FullName.Substring(7), field.Value);
                                    }
                                    else if (field.FullName.Contains(".0") && (!field.FullName.Contains("Weitere") || field.FullName.Contains(".0.")) && !field.FullName.Contains("Englisch.1.0"))
                                    {
                                        dictVersions.Add(field.FullName.Replace(".0", ""), field.Value);
                                    }

                                    #endregion

                                    #region ExtractNiveauUndKenntnisse

                                    if (field.FullName.Contains("Niveau") || field.FullName.Contains("Weitere"))
                                    {
                                        //Get PDF Values from Weitere Kenntnisse
                                        if (field.FullName.Contains("Weitere"))
                                        {
                                            //  Console.WriteLine("WEITERE");
                                            //dictot.Add(field.Value.ToString(), field.FullName ); //dictot["NiveauMigration_2"]);
                                            tempWeitereKennt.Add(field.FullName, field.Value.ToString());
                                        }
                                        else if (field.FullName.Contains("_2") || field.FullName.Contains("_3"))
                                        {
                                            //Get Values from PDF for C#,C++ and other problem PDF values
                                            if (field.FullName.Substring(6).ToLower() == "c_2") kenntnisseNiveau.Add("C#", field.Value.ToString());
                                            else if (field.FullName.Substring(6).ToLower() == "c_3") kenntnisseNiveau.Add("C++", field.Value.ToString());
                                            else if (field.FullName == "NiveauVerdichter_2") kenntnisseNiveau.Add("Verdichter ", field.Value.ToString()); //
                                            else if (field.FullName == "NiveauThermodynamik_2") kenntnisseNiveau.Add("Thermodynamik ", field.Value.ToString());
                                            else if (field.FullName == "NiveauVerfahrenstechnik_2") kenntnisseNiveau.Add("Verfahrenstechnik ", field.Value.ToString());
                                            else if (field.FullName == "NiveauRI Schemata_2") kenntnisseNiveau.Add("R + I Schemata ", field.Value.ToString()); //
                                            else if (field.FullName == "NiveauStrömungstechnik_2") kenntnisseNiveau.Add("Strömungstechnik ", field.Value.ToString()); //
                                            else if (field.FullName == "NiveauFSM Functional Safety Management_2") kenntnisseNiveau.Add("FSM", field.Value.ToString());
                                            else if (field.FullName == "NiveauVerfahrensfließbilder_2") kenntnisseNiveau.Add("Verfahrensfließbilder ", field.Value.ToString()); //


                                            else tempWeitereKennt.Add(field.FullName, field.Value.ToString());
                                            counterForOldFromat++;
                                        }
                                        else
                                        {
                                            kenntnisseNiveau.Add(field.FullName.Substring(6), Convert.ToInt16(field.Value));
                                            counterForOldFromat++;
                                        }
                                    }

                                    #endregion
                                }

                                objCounters[typeName] = (objCounters.ContainsKey(typeName) ? objCounters[typeName] : 0) + 1;
                            }

                            if (counterForOldFromat == 0)
                            {
                                int n;
                                tempWeitereKennt.Clear();
                                foreach (Field field in form.Fields.Values)
                                {
                                    if (field.Value == null)
                                    {
                                        field.Value = "";
                                    }
                                    IntConversion(field.Value.ToString(), out n);
                                    if (field.FullName.Contains("Englisch.1.0"))
                                    {
                                        if (n != 0)
                                        {
                                            kenntnisseNiveau.Add(field.FullName.Replace(".1.0", ""), n);
                                        }
                                    }
                                    if ((field.Value != null && field.Value.ToString().Length > 0) && !field.FullName.ToLower().Contains("name") && (!field.FullName.Contains(".0") && !field.FullName.Contains("Weitere")))
                                    {
                                        if (field.FullName.Contains(".1") && !(field.FullName.Contains("_2") || field.FullName.Contains("_3")))
                                        {
                                            kenntnisseNiveau.Add(field.FullName.Replace(".1", ""), n);
                                        }
                                        else if (field.FullName.Contains("_2") || field.FullName.Contains("_3"))
                                        {
                                            if (field.FullName.ToLower().StartsWith("c_2")) kenntnisseNiveau.Add("C#", field.Value.ToString());
                                            else if (field.FullName.ToLower().StartsWith("c_3")) kenntnisseNiveau.Add("C++", field.Value.ToString());
                                        }
                                        else kenntnisseNiveau.Add(field.FullName, n);
                                    }
                                    if (field.FullName.Contains("Weitere") && field.Value.ToString().Length > 0)
                                    {
                                        //  Console.WriteLine("WEITERE");
                                        //dictot.Add(field.Value.ToString(), field.FullName ); //dictot["NiveauMigration_2"]);
                                        tempWeitereKennt.Add(field.FullName, field.Value.ToString());
                                    }
                                }
                            }

                            if (tempWeitereKennt.Count >= 2)
                            {
                                WeitereProperSort(ref tempWeitereKennt);
                            }

                            //Convert Dictionary with Weitere Kenntnisse to Array, swithch values and add in kenntnisseNiveau Dictionary
                            object[] weitereKenntArray = new object[tempWeitereKennt.Count];
                            tempWeitereKennt.Values.CopyTo(weitereKenntArray, 0);
                            tempWeitereKennt.Clear();
                            for (int i = 0; i < (weitereKenntArray.Length) / 2; i++)
                            {
                                int n;
                                int x;
                                int swapCount = 0;
                                bool isNumeric1 = int.TryParse(weitereKenntArray[i * 2].ToString(), out n);
                                bool isNumeric2 = int.TryParse(weitereKenntArray[i * 2 + 1].ToString(), out x);

                                if (!isNumeric1 && isNumeric2)
                                {
                                    kenntnisseNiveau.Add(weitereKenntArray[i * 2].ToString(), weitereKenntArray[i * 2 + 1]);
                                }
                                else if (swapCount == 0)
                                {
                                    Swap(ref weitereKenntArray, ref swapCount);
                                    i--;
                                }
                            }

                            //int fieldCount = form.Fields.Count;
                            //if (fieldCount == 0)
                            //{ Console.WriteLine("No field available."); }
                            //else
                            //{
                            //    Console.WriteLine("\nFields partial counts (grouped by type):");
                            //    foreach (KeyValuePair<string, int> entry in objCounters)
                            //    { Console.WriteLine(" " + entry.Key + ": " + entry.Value); }
                            //    Console.WriteLine("Fields total count: " + fieldCount);
                            //}
                        }
                    } 
                }
                kenntNiveauVersVorname[0] = vornameUndName;
                
                kenntNiveauVersVorname[1] = DeleteNiveauWithZeroVal(kenntnisseNiveau);

                kenntNiveauVersVorname[2] = dictVersions;
                
            }
            catch (Exception ex)
            {
                Log.ErrorException("Exception in BewerberReadFromPdf\\FormPdfRead", ex);
            }

            return kenntNiveauVersVorname;
        }

        public static Dictionary<string, object> DeleteNiveauWithZeroVal(Dictionary<string, object> kenntnisseNiveau)
        {
            var keysToRemove = kenntnisseNiveau.Where(kvp => kvp.Value.ToString() == "0")
                        .Select(kvp => kvp.Key)
                        .ToArray();

            foreach (var key in keysToRemove)
            {
                kenntnisseNiveau.Remove(key);
            }
            return kenntnisseNiveau;
        }


        public static string ReplaceBadNiveau(string fieldVal)
        {
            switch (fieldVal)
            {
                case "1":
                case "2":
                case "3":
                case "4":
                    break;
                default:
                    fieldVal = "0";
                    break;
            }
            return fieldVal;
        }

        public static void IntConversion(string inputValue , out int n)
        {
            int.TryParse(inputValue, out n);
        }

        public static void Swap(ref object[] array, ref int swapCount)
        {
            try
            {
                object[] kenntnis;
                object[] niveau;

                kenntnis = GetVersion(-1, array);
                niveau = GetVersion(-2, array);

                for (int i = 0; i < array.Length / 2; i++)
                {
                    array[i * 2] = kenntnis[i];
                    array[i * 2 + 1] = niveau[i];
                }
                swapCount++;
            }
            catch (Exception ex)
            {
                  Log.ErrorException("Exception in BewerberReadFromPDF\\Swap", ex);
            }
        }

        public static object[] GetVersion(int x, object[] array)
        {
                object[] arr = new object[0];

                try
                {
                    int j = 0;
                    arr = new object[array.Length / 2];
                    for (int i = 0; i < array.Length; i++)
                    {
                        if(j<arr.Length)
                        {
                            if (array[i].ToString().Length > 1 && x == -1)
                            {
                                arr[j] = array[i];
                                j++;
                            }
                            else if (array[i].ToString().Length == 1 && x == -2)
                            {
                                arr[j] = array[i];
                                j++;
                            }
                            else if (arr[j] == null && x == -2)
                            {
                                arr[j] = 0;
                                j++;
                            } 
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.ErrorException("Exception in BewerberReadFromPDF\\GetVersion", ex);
                }
            return arr;
       }


        public static void WeitereProperSort(ref Dictionary<string, object> tempWeitereKennt)
        {
            try
            {
                string[] arr = {"Weitere Kenntnisse2","Weitere Kenntnisse.0","Weitere Kenntnisse_3","Weitere Kenntnisse.1",
            "Weitere Kenntnisse_4","Weitere Kenntnisse.2","Weitere Kenntnisse_5","Weitere Kenntnisse.3"};

                var newDict = new Dictionary<string, object>();
                for (int i = 0; i < arr.Length / 2; i++)
                {
                    if (tempWeitereKennt.ContainsKey(arr[i * 2]) && tempWeitereKennt.ContainsKey(arr[i * 2 + 1]))
                    {
                        string ken = tempWeitereKennt[arr[i * 2]].ToString();
                        string ken2 = tempWeitereKennt[arr[i * 2 + 1]].ToString();

                        tempWeitereKennt.Remove(arr[i * 2]);
                        tempWeitereKennt.Remove(arr[i * 2 + 1]);

                        newDict.Add(arr[i * 2], ken);
                        newDict.Add(arr[i * 2 + 1], ken2);
                    }
                    else if (tempWeitereKennt.ContainsKey(arr[i * 2]))
                    {
                        tempWeitereKennt.Remove(arr[i * 2]);
                    }
                    else if (tempWeitereKennt.ContainsKey(arr[i * 2]))
                    {
                        tempWeitereKennt.Remove(arr[i * 2]);
                    }
                }

                if (newDict.Count>0)
                {
                    foreach (string key in tempWeitereKennt.Keys)
                    {
                        newDict.Add(key, tempWeitereKennt[key]);
                    }

                    tempWeitereKennt.Clear();
                    tempWeitereKennt = newDict;
                }
                
                //Weitere Kenntnisse2 -- Weitere Kenntnisse.0
                //Weitere Kenntnisse_3 -- Weitere Kenntnisse.1
                //Weitere Kenntnisse_4 -- Weitere Kenntnisse.2
                //Weitere Kenntnisse_5 -- Weitere Kenntnisse.3
            }
            catch (Exception ex)
            {
             Log.ErrorException("Exception in BewerberReadFromPDF\\WeitereProperSoft", ex);
            }
        }
        
        #endregion
    }

}


