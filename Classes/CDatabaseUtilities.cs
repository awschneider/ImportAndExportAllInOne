using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using System.Windows.Forms;

namespace nsImportAndExportAllInOne
{
    class CDatabaseUtilities
    {
        // --------------------------------------------------------------------------------
        // Constants
        // --------------------------------------------------------------------------------

        // --------------------------------------------------------------------------------
        // Properties
        // --------------------------------------------------------------------------------
        private static int intRecordCount = 0;
        private static int[] intRecordAllergyCount;
        private static int[] intRecordAllergyMedicationCount;
        private static int[] intRecordConditionCount;
        private static int[] intRecordConditionMedicationCount;
        private static string[] strPatientExportData;

        private static enuDatabaseTypeType m_enuDatabaseType = enuDatabaseTypeType.SQLServer;

        // In a 2-Tier app we connect once during FMain_Load and hold
        // the connection open while until the application closes
        private static System.Data.OleDb.OleDbConnection m_conAdministrator = null;

        //SQL Server Connection string with integrated login v1
        private static string m_strDatabaseConnectionStringSQLServerV1 = "Provider=SQLOLEDB;" +
                                                                         "Server=(Local);" +
                                                                         "Database=dbPatientAllergies;" +
                                                                         "Integrated Security=SSPI;";

        //SQL Server Connection string with integrated login v2
        private static string m_strDatabaseConnectionStringSQLServerV2 = "Provider=SQLOLEDB;" +
                                                                         "Server=(Local);" +
                                                                         "Database=dbPatientAllergies;" +
                                                                         "Trusted_Connection=True;";

        //SQL Express Connection string
        private static string m_strDatabaseConnectionString = "Provider=SQLOLEDB;" +
                                                              "Server=(Local)\\SQLEXPRESS;" +
                                                              "Database=dbPatientAllergies;" +
                                                              "User ID=sa;" +
                                                              "Password=;";

        //Access 2000 / Windows XP Connection string
        private static string m_strDatabaseConnectionStringMSAccessV1 = "Provider=Microsoft.Jet.OLEDB.4.0;" +
                                                                        "Data Source=" + Application.StartupPath + "\\..\\..\\Database\\TeamsAndPlayers2.mdb;" +
                                                                        "User ID=Admin;" +
                                                                        "Password=;";

        //Access 2007 / Windows 7 Connection string
        private static string m_strDatabaseConnectionStringMSAccessV2 = "Provider=Microsoft.ACE.OLEDB.12.0;" +
                                                                     "Data Source=" + Application.StartupPath + "\\..\\..\\Database\\TeamsAndPlayers2.accdb;" +
                                                                     "User ID=Admin;" +
                                                                     "Password=;";

        #region "Open/Close Connection"

        // --------------------------------------------------------------------------------
        // Name: SetDatabaseType
        // Abstract: Regular or recaf
        // --------------------------------------------------------------------------------
        public static void SetDatabaseType(enuDatabaseTypeType enuDatabaseType)
        {

            try
            {
                m_enuDatabaseType = enuDatabaseType;
            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }

        }



        // --------------------------------------------------------------------------------
        // Name: OpenDatabaseConnection
        // Abstract: Open a connection to the database.
        //           In a 2-Tier (client server) application we connect once in FMain
        //           and hold the connection open until FMain closes
        // --------------------------------------------------------------------------------
        public static bool OpenDatabaseConnection()
        {
            bool blnResult = false;

            try
            {
                switch (m_enuDatabaseType)
                {
                    case enuDatabaseTypeType.MSAccess:
                        blnResult = OpenDatabaseConnectionMSAccessV1();
                        break;

                    case enuDatabaseTypeType.SQLServer:
                        blnResult = OpenDatabaseConnectionSQLServerV1();
                        break;
                }

                // Success
                blnResult = true;
            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }

            return blnResult;
        }



        // --------------------------------------------------------------------------------
        // Name: OpenDatabaseConnectionMSAccess
        // Abstract: Open a connection to the database.
        //           In a 2-Tier (client server) application we connect once in FMain
        //           and hold the connection open until FMain closes
        //
        //           ********** READ ME **********
        //
        //           For MS Access on Windows Vista/7 you must set the target CPU to "x86"
        //           under "Project/Properties/Compiler/Advanced Compil Options (button at bottom)/Target CPU"
        // --------------------------------------------------------------------------------
        public static bool OpenDatabaseConnectionMSAccessV1()
        {
            bool blnResult = false;

            try
            {
                // Open a connection to the database
                m_conAdministrator = new OleDbConnection();
                m_conAdministrator.ConnectionString = m_strDatabaseConnectionStringMSAccessV2;
                m_conAdministrator.Open();

                // Success
                blnResult = true;
            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);

                MessageBox.Show("Unable to connect to the database:" + "\n" +
                                "The application will now close." + "\n" +
                                "\n" +
                                "See CDatabaseUtilities.OpenDatabaseConnection for more details",
                                "Database Connection Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
            }

            return blnResult;
        }



        // --------------------------------------------------------------------------------
        // Name: OpenDatabaseConnectionSQLServer
        // Abstract: Open a connection to the database.
        //           In a 2-Tier (client server) application we connect once in FMain
        //           and hold the connection open until FMain closes
        // --------------------------------------------------------------------------------
        public static bool OpenDatabaseConnectionSQLServerV1()
        {
            bool blnResult = false;

            try
            {
                // Open a connection to the database
                m_conAdministrator = new System.Data.OleDb.OleDbConnection();
                m_conAdministrator.ConnectionString = m_strDatabaseConnectionStringSQLServerV1;
                m_conAdministrator.Open();

                // Success
                blnResult = true;
            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);

                MessageBox.Show("Unable to connect to the database:" + "\n" +
                                "The application will now close." + "\n" +
                                "\n" +
                                "See CDatabaseUtilities.OpenDatabaseConnection for more details",
                                "Database Connection Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
            }

            return blnResult;
        }



        // --------------------------------------------------------------------------------
        // Name: CloseDatabaseConnection
        // Abstract: If the database connection is open then close it.  Release the
        //           memory.
        // --------------------------------------------------------------------------------
        public static bool CloseDatabaseConnection()
        {
            bool blnResult = false;

            try
            {
                // Anything there?
                if (m_conAdministrator != null)
                {
                    // Open?
                    if (m_conAdministrator.State != ConnectionState.Closed)
                    {
                        // Yes, close it
                        m_conAdministrator.Close();
                    }

                    // Clean up
                    m_conAdministrator = null;

                }

                // Success
                blnResult = true;
            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }

            return blnResult;
        }

        #endregion

        #region "Load Lists"

        // --------------------------------------------------------------------------------
        // Name: LoadListBoxFromDatabase
        // Abstract: Load a Listbox control from a table in the Datbase
        // --------------------------------------------------------------------------------
        public static bool LoadListBoxFromDatabase(string strTableName,
                                            string strPrimaryKey,
                                            string strNameColumn,
                                            ref ListBox lstTargetListBox,
                                            string strSortColumn = "",
                                            string strCustomSQL = "")
        {
            bool blnResult = false;

            try
            {
                string strSelect = "";
                OleDbCommand cmdSelect = null;
                OleDbDataReader drSourceTable = null;
                CListItem liNewItem = null;
                int intID = 0;
                string strName = "";

                // Show changes all at once at the end (Much faster)
                lstTargetListBox.BeginUpdate();

                // Clear out the list
                lstTargetListBox.Items.Clear();

                strSelect = BuildSelectStatement(strTableName, strPrimaryKey,
                                                 strNameColumn, strSortColumn,
                                                 strCustomSQL);

                // Wrap a command object around the select statement
                cmdSelect = new OleDbCommand(strSelect, m_conAdministrator);

                // retrive all the records from the database
                drSourceTable = cmdSelect.ExecuteReader();

                // Data Readers start reading at first row. Move the DataReader's (Row) or position by calling .Read() once.
                // First row is headers? intTeamID, strTeam?
                drSourceTable.Read();

                // loop through records one at a time
                // Each call to read moves to the next record
                while (drSourceTable.Read() == true)
                {
                    // Make a listItem to hold the information
                    intID = (int)Conversion.Val(drSourceTable[0]);
                    strName = drSourceTable[1].ToString();
                    liNewItem = new CListItem(intID, strName);


                    //liNewItem(intID, strName);

                    // Add the item to the list
                    lstTargetListBox.Items.Add(liNewItem);
                }

                if (lstTargetListBox.Items.Count > 0) lstTargetListBox.SelectedIndex = 0;

                // Show nay changes
                lstTargetListBox.EndUpdate();

                // Clean up
                drSourceTable.Close();

                // Success
                blnResult = true;

            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }

            return blnResult;

        }



        // --------------------------------------------------------------------------------
        // Name: LoadComboBoxFromDatabase
        // Abstract: Load a Combobox control from a table in the Datbase
        // --------------------------------------------------------------------------------
        public static bool LoadComboBoxFromDatabase(string strTableName,
                                            string strPrimaryKey,
                                            string strNameColumn,
                                            ref ComboBox cmbTarget,
                                            string strSortColumn = "",
                                            string strCustomSQL = "")
        {
            bool blnResult = false;

            try
            {
                string strSelect = "";
                OleDbCommand cmdSelect = null;
                OleDbDataReader drSourceTable = null;
                CListItem liNewItem = null;
                int intID = 0;
                string strName = "";

                // Show changes all at once at the end (Much faster)
                cmbTarget.BeginUpdate();

                // Clear out the list
                cmbTarget.Items.Clear();

                strSelect = BuildSelectStatement(strTableName, strPrimaryKey,
                                                 strNameColumn, strSortColumn,
                                                 strCustomSQL);

                // Wrap a command object around the select statement
                cmdSelect = new OleDbCommand(strSelect, m_conAdministrator);

                // retrive all the records from the database
                drSourceTable = cmdSelect.ExecuteReader();

                // Move it one spot
                drSourceTable.Read();

                // loop through records one at a time
                // Each call to read moves to the next record
                do
                {
                    // Make a listItem to hold the information
                    intID = (int)Conversion.Val(drSourceTable[0]);
                    strName = (string)drSourceTable[1];
                    liNewItem = new CListItem(intID, strName);

                    //liNewItem(intID, strName);

                    // Add the item to the list
                    cmbTarget.Items.Add(liNewItem);
                }
                while (drSourceTable.Read() == true);

                // Select the first item in the list by default
                if (cmbTarget.Items.Count > 0) cmbTarget.SelectedIndex = 0;

                // Show any changes
                cmbTarget.EndUpdate();

                // Clean up
                drSourceTable.Close();

                // Success
                blnResult = true;

            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }

            return blnResult;

        }



        // --------------------------------------------------------------------------------
        // Name: BuildSelectStatement
        // Abstract: Build a select state for the table, ID and Name column.
        // --------------------------------------------------------------------------------
        private static string BuildSelectStatement(string strTable, string strPrimaryKey,
                                                   string strNameColumn, string strSortColumn,
                                                   string strCustomSQL)
        {
            string strSelectStatement = "";

            try
            {
                // Custom select statement?
                if (strCustomSQL == "")
                {
                    // No, so build one

                    // Sort by name column if nothing provided 
                    if (strSortColumn == "") strSortColumn = strNameColumn;

                    // Put it all together
                    strSelectStatement = "SELECT " +
                                         strPrimaryKey + ", " + strNameColumn +
                                         " FROM " +
                                         strTable +
                                         " ORDER BY " + strSortColumn;
                }
                else
                {
                    strSelectStatement = strCustomSQL;
                }

            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }

            return strSelectStatement;
        }



        // --------------------------------------------------------------------------------
        // Name: LoadListViewFromDatabase
        // Abstract: Load a Listbox control from a table in the Datbase
        // --------------------------------------------------------------------------------
        public static bool LoadListViewFromDatabase(string strTableName,
                                            string strPrimaryKey,
                                            string[] astrNameColumns,
                                            ref ListView lvwTarget,
                                            string strSortColumn = "",
                                            string strCustomSQL = "")
        {
            bool blnResult = false;

            try
            {
                string strSelect = "";
                OleDbCommand cmdSelect = null;
                OleDbDataReader drSourceTable = null;
                ListViewItem lviNewItem = null;
                int intColumnIndex = 0;
                string strColumn = "";

                // Show changes all at once at the end (Much faster)
                lvwTarget.BeginUpdate();

                strSelect = BuildSelectStatement(strTableName, strPrimaryKey,
                                                 astrNameColumns, strSortColumn,
                                                 strCustomSQL);

                // Wrap a command object around the select statement
                cmdSelect = new OleDbCommand(strSelect, m_conAdministrator);

                // retrive all the records from the database
                drSourceTable = cmdSelect.ExecuteReader();

                // loop through records one at a time
                // Each call to read moves to the next record
                while (drSourceTable.Read() == true)
                {
                    // Make a list view item to hold the information
                    lviNewItem = new ListViewItem();
                    lviNewItem.Tag = drSourceTable[0];              // Primary Key
                    lviNewItem.Text = (string)drSourceTable[1];     // Name Column

                    for (intColumnIndex = 2; intColumnIndex < drSourceTable.FieldCount; intColumnIndex += 1)
                    {
                        // One at at time
                        strColumn = drSourceTable[intColumnIndex].ToString();
                        lviNewItem.SubItems.Add(strColumn);
                    }

                    // add the item to the list
                    lvwTarget.Items.Add(lviNewItem);
                }


                // Select the first Item in the list
                if (lvwTarget.Items.Count > 0) lvwTarget.Items[0].Selected = true;

                // Show any changes
                lvwTarget.EndUpdate();

                // Clean up
                drSourceTable.Close();

                // Success
                blnResult = true;

            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }

            return blnResult;

        }


        // --------------------------------------------------------------------------------
        // Name: BuildSelectStatement
        // Abstract: Build a select state for the table, ID and Name column.
        // --------------------------------------------------------------------------------
        private static string BuildSelectStatement(string strTable,
                                            string strPrimaryKey,
                                            string[] astrNameColumns,
                                            string strSortColumn,
                                            string strCustomSQL)
        {
            string strSelectStatement = "";

            try
            {
                int intIndex = 0;
                string strAllNameColumns = "";

                // Custom select statement?
                if (strCustomSQL == "")
                {
                    // No, so build one
                    // Get all the name columns
                    for (intIndex = 0; intIndex < astrNameColumns.Length; intIndex += 1)
                    {
                        strAllNameColumns += ", " + astrNameColumns[intIndex];
                    }

                    // Remove leading comma
                    strAllNameColumns = strAllNameColumns.Substring(2);

                    // Sort by name column if nothing provided
                    if (strSortColumn == "")
                    {
                        strSortColumn = strAllNameColumns;

                        // Put it all together
                        strSelectStatement = "SELECT " +
                                             strPrimaryKey + ", " + strAllNameColumns +
                                             " FROM " +
                                             strTable +
                                             " ORDER BY " + strSortColumn;
                    }
                    else
                    {
                        strSelectStatement = strCustomSQL;
                    }
                }

            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }

            return strSelectStatement;
        }

        #endregion

        #region "Delete/GetNextHighestID"

        // --------------------------------------------------------------------------------
        // Name: DeleteRecordsFromTable
        // Abstract: Delete all records from table that match the ID.
        // --------------------------------------------------------------------------------
        public static bool DeleteRecordsFromTable(int intRecordID,
                                           string strPrimaryKey,
                                           string strTable,
                                           string strCustomSQL = "")
        {
            bool blnResult = false;

            try
            {

                string strDelete = "";
                OleDbCommand cmdDelete = null;
                int intRowsAffected = 0;

                // Custom SQL?
                if (strCustomSQL != "")
                {
                    // Yes, use it
                    strDelete = strCustomSQL;
                }
                else
                {
                    // No, build the SQL String
                    strDelete = "DELETE FROM " + strTable +
                                " WHERE " + strPrimaryKey + " = " + intRecordID;
                }

                // Delete the record(s)
                cmdDelete = new OleDbCommand(strDelete, m_conAdministrator);
                intRowsAffected = cmdDelete.ExecuteNonQuery();

                // Did it work?
                if (intRowsAffected > 0)
                {
                    // Yes, success
                    blnResult = true;
                }

            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }

            return blnResult;
        }



        // --------------------------------------------------------------------------------
        // Name: GetNextHighestRecordID
        // Abstract: Get the next highest ID from the table in the database
        // Danger: Potential race condition.
        // Why do we have this? So we can see the mechanics of how
        // everthing works.
        // --------------------------------------------------------------------------------
        public static bool GetNextHighestRecordID(string strPrimaryKey, string strTable, ref int intNextHighestRecordID)
        {
            bool blnResult = false;

            try
            {
                string strSelect = "";
                OleDbCommand cmdSelect = null;
                OleDbDataReader drSourceTable = null;

                strSelect = "SELECT MAX( " + strPrimaryKey + " ) + 1 AS intNextHighestRecordID " +
                            " FROM " + strTable;

                // Execute command
                cmdSelect = new OleDbCommand(strSelect, m_conAdministrator);
                drSourceTable = cmdSelect.ExecuteReader();

                // Read result( highest ID )
                drSourceTable.Read();

                // Null? (empty table)
                if (drSourceTable.IsDBNull(0) == true)
                {
                    // Yes, start numbering at 1
                    intNextHighestRecordID = 1;
                }
                else
                {
                    // No, get the next highest ID
                    intNextHighestRecordID = (int)drSourceTable[0];
                }

                // Clean up
                drSourceTable.Close();

                // Success
                blnResult = true;
            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }

            return blnResult;
        }

        #endregion

        #region "IMPORT"

        // --------------------------------------------------------------------------------
        // Name: GetStateID
        // Abstract: Get the state ID from the provided abbreviation
        // --------------------------------------------------------------------------------
        public static bool GetStateID(string strState, ref int intStateID)
        {
            bool blnResult = false;

            try
            {
                OleDbCommand cmdSelect = null;
                OleDbDataReader drSourceTable = null;
                string strSelect = "SELECT * FROM TStates " +
                                   "WHERE strStateAbbreviation = " + "'" + strState + "'";

                // Execute command
                cmdSelect = new OleDbCommand(strSelect, m_conAdministrator);
                drSourceTable = cmdSelect.ExecuteReader();

                // Read result
                drSourceTable.Read();

                // Was it Valid?
                if (drSourceTable.HasRows)
                {
                    intStateID = (int)drSourceTable[0];

                    // Success
                    blnResult = true;
                }

                // Clean up
                drSourceTable.Close();

            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }

            return blnResult;
        }


        #region "Allergies"

        // --------------------------------------------------------------------------------
        // Name: GetAllergyID
        // Abstract: Get the Allergy ID from the text
        // --------------------------------------------------------------------------------
        public static bool GetAllergyID(ref udtAllergyType udtAllergy)
        {
            bool blnResult = false;

            try
            {
                OleDbCommand cmdStoredProcedure = null;
                OleDbDataReader drReturnValues = null;

                // Execute command
                cmdStoredProcedure = new OleDbCommand("uspGetOrAddAllergy", m_conAdministrator);
                cmdStoredProcedure.CommandType = CommandType.StoredProcedure;

                // Add parameters
                cmdStoredProcedure.Parameters.AddWithValue("1", udtAllergy.strAllergy);
                // Execute the stored procedure
                drReturnValues = cmdStoredProcedure.ExecuteReader();

                // Read Result
                drReturnValues.Read();

                // Get the new ID (could also use an output parameter)
                udtAllergy.intAllergyID = (int)drReturnValues[0];

                // Clean Up
                drReturnValues.Close();

                // Success
                blnResult = true;
            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }

            return blnResult;
        }


        // --------------------------------------------------------------------------------
        // Name: AssignPatientAllergyIntoDatabase
        // Abstract: Assign a Patient Allergy
        // --------------------------------------------------------------------------------
        public static bool AssignPatientAllergyInDatabase(udtPatientType udtPatient, udtAllergyType udtAllergy)
        {
            bool blnResult = false;

            try
            {
                OleDbCommand cmdStoredProcedure = null;
                OleDbDataReader drReturnValues = null;

                // Execute command
                cmdStoredProcedure = new OleDbCommand("uspAddPatientAllergy", m_conAdministrator);
                cmdStoredProcedure.CommandType = CommandType.StoredProcedure;

                // Add parameters
                cmdStoredProcedure.Parameters.AddWithValue("1", udtPatient.intPatientID);
                cmdStoredProcedure.Parameters.AddWithValue("2", udtAllergy.intAllergyID);

                // Execute the stored procedure
                drReturnValues = cmdStoredProcedure.ExecuteReader();

                // Clean up
                drReturnValues.Close();

                // Success
                blnResult = true;

            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }

            return blnResult;
        }
        #endregion

        #region "Medications"
        // --------------------------------------------------------------------------------
        // Name: GetMedicationID
        // Abstract: Get the Medication ID from the text
        // --------------------------------------------------------------------------------
        public static bool GetMedicationID(ref udtMedicationType udtMedication)
        {
            bool blnResult = false;

            try
            {
                OleDbCommand cmdStoredProcedure = null;
                OleDbDataReader drReturnValues = null;

                // Execute command
                cmdStoredProcedure = new OleDbCommand("uspGetOrAddMedication", m_conAdministrator);
                cmdStoredProcedure.CommandType = CommandType.StoredProcedure;

                // Add parameters
                cmdStoredProcedure.Parameters.AddWithValue("1", udtMedication.strMedication);
                // Execute the stored procedure
                drReturnValues = cmdStoredProcedure.ExecuteReader();

                // Read Result
                drReturnValues.Read();

                // Get the new ID (could also use an output parameter)
                udtMedication.intMedicationID = (int)drReturnValues[0];

                // Clean Up
                drReturnValues.Close();

                // Success
                blnResult = true;
            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }

            return blnResult;
        }


        // --------------------------------------------------------------------------------
        // Name: AssignPatientAllergyMedication
        // Abstract: Assign a Patient Allergy
        // --------------------------------------------------------------------------------
        public static bool AssignPatientAllergyMedication(udtPatientType udtPatient, udtAllergyType udtAllergy, udtMedicationType udtMedication)
        {
            bool blnResult = false;

            try
            {
                OleDbCommand cmdStoredProcedure = null;
                OleDbDataReader drReturnValues = null;

                // Execute command
                cmdStoredProcedure = new OleDbCommand("uspAddPatientAllergyMedication", m_conAdministrator);
                cmdStoredProcedure.CommandType = CommandType.StoredProcedure;

                // Add parameters
                cmdStoredProcedure.Parameters.AddWithValue("1", udtPatient.intPatientID);
                cmdStoredProcedure.Parameters.AddWithValue("2", udtAllergy.intAllergyID);
                cmdStoredProcedure.Parameters.AddWithValue("3", udtMedication.intMedicationID);

                if (NotDupicatePatientAllergyMedicationData(udtPatient, udtAllergy, udtMedication) == true)
                {
                    // Execute the stored procedure
                    drReturnValues = cmdStoredProcedure.ExecuteReader();

                    // Success
                    blnResult = true;

                    // Clean up
                    drReturnValues.Close();
                }

            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }

            return blnResult;
        }



        // --------------------------------------------------------------------------------
        // Name: NotDupicatePatientAllergyMedicationData
        // Abstract: Checks for Duplication Patient Allergy Medication data
        // --------------------------------------------------------------------------------
        public static bool NotDupicatePatientAllergyMedicationData(udtPatientType udtPatient, udtAllergyType udtAllergy, udtMedicationType udtMedication)
        {
            bool blnResult = false;

            try
            {
                OleDbCommand cmdSelect = null;
                OleDbDataReader drSourceTable = null;
                string strSelect = "SELECT * FROM TPatientAllergyMedications" +
                                   " WHERE intPatientID = " + udtPatient.intPatientID + " AND " +
                                          "intAllergyID = " + udtAllergy.intAllergyID + " AND " +
                                          "intMedicationID = " + udtMedication.intMedicationID;

                // Execute command
                cmdSelect = new OleDbCommand(strSelect, m_conAdministrator);

                // Execute the stored procedure
                drSourceTable = cmdSelect.ExecuteReader();

                // Execute the stored procedure
                drSourceTable.Read();

                // Is there anything
                if (drSourceTable.HasRows == false)
                {
                    // No, GOOD!
                    blnResult = true;
                }

                // Clean up
                drSourceTable.Close();
            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }

            return blnResult;
        }


        #endregion

        #region "Procedures"


        // --------------------------------------------------------------------------------
        // Name: GetProcedureID
        // Abstract: Get the Procedure ID from the text
        // --------------------------------------------------------------------------------
        public static bool GetProcedureID(ref udtProcedureType udtProcedure)
        {
            bool blnResult = false;

            try
            {
                OleDbCommand cmdStoredProcedure = null;
                OleDbDataReader drReturnValues = null;

                // Execute command
                cmdStoredProcedure = new OleDbCommand("uspGetOrAddProcedure", m_conAdministrator);
                cmdStoredProcedure.CommandType = CommandType.StoredProcedure;

                // Add parameters
                cmdStoredProcedure.Parameters.AddWithValue("1", udtProcedure.strProcedure);
                // Execute the stored procedure
                drReturnValues = cmdStoredProcedure.ExecuteReader();

                // Read Result
                drReturnValues.Read();

                // Get the new ID (could also use an output parameter)
                udtProcedure.intProcedureID = (int)drReturnValues[0];

                // Clean Up
                drReturnValues.Close();

                // Success
                blnResult = true;
            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }

            return blnResult;
        }



        // --------------------------------------------------------------------------------
        // Name: AssignPatientProcedure
        // Abstract: Assign a Patient Allergy
        // --------------------------------------------------------------------------------
        public static bool AssignPatientProcedure(udtPatientType udtPatient, udtProcedureType udtProcedure)
        {
            bool blnResult = false;

            try
            {
                OleDbCommand cmdStoredProcedure = null;
                OleDbDataReader drReturnValues = null;

                // Execute command
                cmdStoredProcedure = new OleDbCommand("uspAddPatientProcedure", m_conAdministrator);
                cmdStoredProcedure.CommandType = CommandType.StoredProcedure;

                // Add parameters
                cmdStoredProcedure.Parameters.AddWithValue("1", udtPatient.intPatientID);
                cmdStoredProcedure.Parameters.AddWithValue("2", udtProcedure.intProcedureID);

                // Execute the stored procedure
                drReturnValues = cmdStoredProcedure.ExecuteReader();

                // Clean up
                drReturnValues.Close();

                // Success
                blnResult = true;

            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }

            return blnResult;
        }

        #endregion

        #region "Record Creators"

        // --------------------------------------------------------------------------------
        // Name: GetRecordCreatorID
        // Abstract: Get the Allergy ID from the text
        // --------------------------------------------------------------------------------
        public static bool GetRecordCreatorID(ref udtRecordCreatorType udtRecordCreator)
        {
            bool blnResult = false;

            try
            {
                OleDbCommand cmdStoredProcedure = null;
                OleDbDataReader drReturnValues = null;

                // Execute command
                cmdStoredProcedure = new OleDbCommand("uspGetOrAddRecordCreator", m_conAdministrator);
                cmdStoredProcedure.CommandType = CommandType.StoredProcedure;

                // Add parameters
                cmdStoredProcedure.Parameters.AddWithValue("1", udtRecordCreator.strRecordCreator);
                // Execute the stored procedure
                drReturnValues = cmdStoredProcedure.ExecuteReader();

                // Read Result
                drReturnValues.Read();

                // Get the new ID (could also use an output parameter)
                udtRecordCreator.intRecordCreatorID = (int)drReturnValues[0];

                // Clean Up
                drReturnValues.Close();

                // Success
                blnResult = true;
            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }

            return blnResult;
        }
        #endregion

        #region "Conditions"


        // --------------------------------------------------------------------------------
        // Name: GetConditionID
        // Abstract: Get the Condition ID from the text
        // --------------------------------------------------------------------------------
        public static bool GetConditionID(ref udtConditionType udtCondition)
        {
            bool blnResult = false;

            try
            {
                OleDbCommand cmdStoredProcedure = null;
                OleDbDataReader drReturnValues = null;

                // Execute command
                cmdStoredProcedure = new OleDbCommand("uspGetOrAddCondition", m_conAdministrator);
                cmdStoredProcedure.CommandType = CommandType.StoredProcedure;

                // Add parameters
                cmdStoredProcedure.Parameters.AddWithValue("1", udtCondition.strCondition);
                // Execute the stored procedure
                drReturnValues = cmdStoredProcedure.ExecuteReader();

                // Read Result
                drReturnValues.Read();

                // Get the new ID (could also use an output parameter)
                udtCondition.intConditionID = (int)drReturnValues[0];

                // Clean Up
                drReturnValues.Close();

                // Success
                blnResult = true;
            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }

            return blnResult;
        }


        // --------------------------------------------------------------------------------
        // Name: AssignPatientConditionInDatabase
        // Abstract: Assign a Patient Condition
        // --------------------------------------------------------------------------------
        public static bool AssignPatientConditionInDatabase(udtPatientType udtPatient, udtConditionType udtCondition)
        {
            bool blnResult = false;

            try
            {
                OleDbCommand cmdStoredProcedure = null;
                OleDbDataReader drReturnValues = null;

                // Execute command
                cmdStoredProcedure = new OleDbCommand("uspAddPatientCondition", m_conAdministrator);
                cmdStoredProcedure.CommandType = CommandType.StoredProcedure;

                // Add parameters
                cmdStoredProcedure.Parameters.AddWithValue("1", udtPatient.intPatientID);
                cmdStoredProcedure.Parameters.AddWithValue("2", udtCondition.intConditionID);

                // Execute the stored Condition
                drReturnValues = cmdStoredProcedure.ExecuteReader();

                // Clean up
                drReturnValues.Close();

                // Success
                blnResult = true;

            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }

            return blnResult;
        }



        // --------------------------------------------------------------------------------
        // Name: AssignPatientConditionMedication
        // Abstract: Assign a Patient Condition
        // --------------------------------------------------------------------------------
        public static bool AssignPatientConditionMedication(udtPatientType udtPatient, udtConditionType udtCondition, udtMedicationType udtMedication)
        {
            bool blnResult = false;

            try
            {
                OleDbCommand cmdStoredProcedure = null;
                OleDbDataReader drReturnValues = null;

                // Execute command
                cmdStoredProcedure = new OleDbCommand("uspAddPatientConditionMedication", m_conAdministrator);
                cmdStoredProcedure.CommandType = CommandType.StoredProcedure;

                // Add parameters
                cmdStoredProcedure.Parameters.AddWithValue("1", udtPatient.intPatientID);
                cmdStoredProcedure.Parameters.AddWithValue("2", udtCondition.intConditionID);
                cmdStoredProcedure.Parameters.AddWithValue("3", udtMedication.intMedicationID);

                // Execute the stored procedure
                drReturnValues = cmdStoredProcedure.ExecuteReader();

                // Clean up
                drReturnValues.Close();

                // Success
                blnResult = true;

            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }

            return blnResult;
        }

        #endregion



        // --------------------------------------------------------------------------------
        // Name: AddPatientToDatabase
        // Abstract: Adding a patient to a Database with a Stored procedure
        // --------------------------------------------------------------------------------
        public static bool AddPatientToDatabase(ref udtPatientType udtPatient)
        {
            bool blnResult = false;

            try
            {
                OleDbCommand cmdStoredProcedure = null;
                OleDbDataReader drReturnValues = null;
                string strMinimumSQLDate = SqlDateTime.MinValue.ToString();

                if (GetNextHighestRecordID("intPatientID", "TPatients", ref udtPatient.intPatientID) == true)
                {
                    // Create sqlcommand object to run our stored procedure
                    cmdStoredProcedure = new OleDbCommand("uspAddPatient", m_conAdministrator);
                    cmdStoredProcedure.CommandType = CommandType.StoredProcedure;

                    // Add parameters
                    cmdStoredProcedure.Parameters.Add(new OleDbParameter("@strFirstName", udtPatient.strFirstName));
                    cmdStoredProcedure.Parameters.Add(new OleDbParameter("@strMiddleName", udtPatient.strMiddleName));
                    cmdStoredProcedure.Parameters.Add(new OleDbParameter("@strLastName", udtPatient.strLastName));
                    cmdStoredProcedure.Parameters.Add(new OleDbParameter("@strStreetAddress", udtPatient.strStreetAddress));
                    cmdStoredProcedure.Parameters.Add(new OleDbParameter("@strCity", udtPatient.strCity));
                    cmdStoredProcedure.Parameters.Add(new OleDbParameter("@intStateID", udtPatient.intStateID));
                    cmdStoredProcedure.Parameters.Add(new OleDbParameter("@strZipCode", udtPatient.strZipCode));
                    cmdStoredProcedure.Parameters.Add(new OleDbParameter("@strPrimaryPhoneNumber", udtPatient.strPrimaryPhoneNumber));
                    cmdStoredProcedure.Parameters.Add(new OleDbParameter("@strSecondaryPhoneNumber", udtPatient.strSecondaryPhoneNumber));
                    cmdStoredProcedure.Parameters.Add(new OleDbParameter("@blnSmoker", udtPatient.blnSmoker));
                    cmdStoredProcedure.Parameters.Add(new OleDbParameter("@decPackYears", udtPatient.decPackYears));
                    cmdStoredProcedure.Parameters.Add(new OleDbParameter("@blnHeadOgHouseHold", udtPatient.blnHeadOfHouseHold));
                    cmdStoredProcedure.Parameters.Add(new OleDbParameter("@dtmDateOfBirth", udtPatient.dtmDateOfBirth));
                    cmdStoredProcedure.Parameters.Add(new OleDbParameter("@intSexID", udtPatient.intSexID));
                    cmdStoredProcedure.Parameters.Add(new OleDbParameter("@intRecordCreatorID", udtPatient.intRecordCreatorID));
                    cmdStoredProcedure.Parameters.Add(new OleDbParameter("@strEmailAddress", udtPatient.strEmailAddress));

                    // Execute the stored procedure
                    drReturnValues = cmdStoredProcedure.ExecuteReader();

                    // Should be 1 and only 1 record returned
                    drReturnValues.Read();

                    // Get the new ID (could also use an output parameter)
                    udtPatient.intPatientID = (int)Conversion.Val(drReturnValues["intPatientID"].ToString());

                    // Clean up
                    drReturnValues.Close();

                    // Success
                    blnResult = true;
                }
            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }

            return blnResult;
        }


        #endregion


        #region "EXPORT"

        // --------------------------------------------------------------------------------
        // Name: NumberofRecordsInTable
        // Absteract: returns the number of records in a particular table.
        // *** SETS THE FORM LEVEL VARIABLE***
        // --------------------------------------------------------------------------------
        public static int NumberofRecordsInTable(string strTable)
        {

            try
            {
                OleDbCommand cmdSelect = null;
                OleDbDataReader drSourceTable = null;
                string strSelect = "SELECT COUNT(*) FROM " + strTable;

                // Execute command
                cmdSelect = new OleDbCommand(strSelect, m_conAdministrator);
                drSourceTable = cmdSelect.ExecuteReader();

                // Read result
                drSourceTable.Read();

                // Was it Valid?
                if (drSourceTable.HasRows)
                {
                    intRecordCount = (int)drSourceTable[0];
                }
            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }

            return intRecordCount;
        }



        // --------------------------------------------------------------------------------
        // Name: NumberofRecordsInTable
        // Absteract: returns the number of records in a particular table.
        // *** SETS THE PASSED VARIABLE***
        // --------------------------------------------------------------------------------
        public static int NumberofRecordsInTable(string strTable, ref int intRecordCount)
        {

            try
            {
                OleDbCommand cmdSelect = null;
                OleDbDataReader drSourceTable = null;
                string strSelect = "SELECT COUNT(*) FROM " + strTable;

                // Execute command
                cmdSelect = new OleDbCommand(strSelect, m_conAdministrator);
                drSourceTable = cmdSelect.ExecuteReader();

                // Read result
                drSourceTable.Read();

                // Was it Valid?
                if (drSourceTable.HasRows)
                {
                    intRecordCount = (int)drSourceTable[0];
                }
            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }

            return intRecordCount;
        }



        // --------------------------------------------------------------------------------
        // Name: NumberofRecordsInTable
        // Absteract: returns the number of records in a particular table
        // --------------------------------------------------------------------------------
        public static int PatientAllergyCount(string strTable)
        {

            try
            {

                int intAllergyRecordCount = 0;
                OleDbCommand cmdSelect = null;
                OleDbDataReader drSourceTable = null;
                string strSelect = "SELECT * FROM VPatientAllergyCount";

                // Execute command
                cmdSelect = new OleDbCommand(strSelect, m_conAdministrator);
                drSourceTable = cmdSelect.ExecuteReader();

                // Read result
                drSourceTable.Read();

                // Was it Valid?
                if (drSourceTable.HasRows)
                {
                    intRecordCount = (int)drSourceTable[0];
                }
            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }

            return intRecordCount;
        }



        // --------------------------------------------------------------------------------
        // Name: NumberofRecordsInTable
        // Absteract: returns the number of records in a particular table
        // --------------------------------------------------------------------------------
        public static void PatientData()
        {

            try
            {
                OleDbCommand cmdSelect = null;
                OleDbDataReader drSourceTable = null;
                string strSelect = "SELECT * FROM VPatientsData";
                int intIndex = 0;

                // Execute command
                cmdSelect = new OleDbCommand(strSelect, m_conAdministrator);
                drSourceTable = cmdSelect.ExecuteReader();

                // Read result
                drSourceTable.Read();

                // Was it Valid?
                if (drSourceTable.HasRows)
                {
                    for(intIndex = 0; intIndex < intRecordCount - 1; intIndex += 1)
                    {
                        strPatientExportData[intIndex] = (string)drSourceTable[0];
                        drSourceTable.Read();
                    } 
                }
            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }

        }

        #endregion






        #region "TTeams"

        // ----------------------------------------------------------------------------------------------------
        // ----------------------------------------------------------------------------------------------------
        // Teams
        // ----------------------------------------------------------------------------------------------------
        // ----------------------------------------------------------------------------------------------------



        // --------------------------------------------------------------------------------
        // Name: AddTeamToDatabase
        // Abstract: Add the team to the database
        // --------------------------------------------------------------------------------
        public static bool AddTeamToDatabase(ref udtTeamType udtTeam)
        {
            bool blnResult = false;

            try
            {
                // Database type?
                switch (m_enuDatabaseType)
                {
                    // MS Access
                    case enuDatabaseTypeType.MSAccess:
                        blnResult = AddTeamToDatabase1(udtTeam);
                        break;
                    case enuDatabaseTypeType.SQLServer:
                        blnResult = AddTeamToDatabase2(ref udtTeam);
                        break;

                }
            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }

            return blnResult;
        }



        // --------------------------------------------------------------------------------
        // Name: AddTeamToDatabase1
        // Abstract: Add the team to the database
        // --------------------------------------------------------------------------------
        private static bool AddTeamToDatabase1(udtTeamType udtTeam)
        {
            bool blnResult = false;

            try
            {
                string strInsert = "";
                OleDbCommand cmdInsert = null;

                // Get the next highest team ID
                // Race condition. Need an atomic action but not possible in access.

                if (GetNextHighestRecordID("intTeamID", "TTeams", ref udtTeam.intTeamID) == true)
                {
                    // Build the INSERT command. Never build command with raw user input to prevent SQL injections.
                    strInsert = "INSERT INTO TTeams ( intTeamID, strTeam, strMascot, intTeamStatusID )" +
                                " VALUES ( ?, ?, ?, ? )";

                    // Make the command instance
                    cmdInsert = new OleDbCommand(strInsert, m_conAdministrator);

                    // Add column values here instead of above to prevent SQL injection attacks
                    cmdInsert.Parameters.AddWithValue("1", udtTeam.intTeamID);
                    cmdInsert.Parameters.AddWithValue("2", udtTeam.strTeam);
                    cmdInsert.Parameters.AddWithValue("3", udtTeam.strMascot);

                    // Insert the row
                    cmdInsert.ExecuteNonQuery();

                    // Success
                    blnResult = true;
                }

            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }

            return blnResult;

        }




        // --------------------------------------------------------------------------------
        // Name: AddTeamToDatabase2
        // Abstract: How to add a record using a stored procedure that returns
        // the record ID. Use this for SQL Server.
        // Advantages:
        // 1) There is only one back and forth from the code to the database.
        // 2) Using a stored procedure takes much of the SQL out of our
        // code and puts it in the database.
        // 3) Stored procedures are guaranteed to be syntactically correct
        // once created (unless you are doing dynamic queries)
        // 4) Stored procedures are pre-compiled (after first run and then cached)
        // so they execute as quickly as possible.
        // 5) Once started the stored procedure is guaranteed to finish
        // without any further input which is good because we will never
        // have an uncommited transaction.
        // --------------------------------------------------------------------------------
        private static bool AddTeamToDatabase2(ref udtTeamType udtTeam)
        {
            bool blnResult = false;

            try
            {
                OleDbCommand cmdStoredProcedure = null;
                OleDbDataReader drReturnValues = null;


                // Create sqlcommand object to run our stored procedure
                cmdStoredProcedure = new OleDbCommand("uspAddTeam", m_conAdministrator);
                cmdStoredProcedure.CommandType = CommandType.StoredProcedure;

                // Add parameters
                cmdStoredProcedure.Parameters.AddWithValue("1", udtTeam.strTeam);
                cmdStoredProcedure.Parameters.AddWithValue("2", udtTeam.strMascot);

                // Execute the stored procedure
                drReturnValues = cmdStoredProcedure.ExecuteReader();

                // Should be 1 and only 1 record returned
                drReturnValues.Read();

                // Get the new ID (could also use an output parameter)
                udtTeam.intTeamID = drReturnValues.GetOrdinal("intTeamID");

                // Clean up
                drReturnValues.Close();

                // Success
                blnResult = true;
            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }

            return blnResult;
        }



        // --------------------------------------------------------------------------------
        // Name: GetTeamInformationFromDatabase
        // Abstract: Get data for the specified team from the database
        // --------------------------------------------------------------------------------
        public static bool GetTeamInformationFromDatabase(ref udtTeamType udtTeam)
        {
            bool blnResult = false;
            try
            {
                string strSelect = "";
                OleDbCommand cmdSelect = null;
                OleDbDataReader drTTeams = null;

                // Build the select string
                strSelect = "SELECT *" +
                            " FROM VActiveTeams" +
                            " WHERE intTeamID = " + udtTeam.intTeamID;

                // Retrieve the record
                cmdSelect = new OleDbCommand(strSelect, m_conAdministrator);

                drTTeams = cmdSelect.ExecuteReader();

                // Read (there should be 1 and only 1 row)
                drTTeams.Read();

                udtTeam.strTeam = drTTeams["strTeam"].ToString();
                udtTeam.strMascot = drTTeams["strMascot"].ToString();

                // Clean up
                drTTeams.Close();

                // Success
                blnResult = true;
            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }

            return blnResult;
        }



        // --------------------------------------------------------------------------------
        // Name: EditTeamInDatabase
        // Abstract: Edit the team in the database
        // --------------------------------------------------------------------------------
        public static bool EditTeamInDatabase(ref udtTeamType udtTeam)
        {
            bool blnResult = false;

            try
            {
                // Database type?
                switch (m_enuDatabaseType)
                {
                    // MS Access
                    case enuDatabaseTypeType.MSAccess:
                        blnResult = EditTeamInDatabase1(udtTeam);
                        break;
                    // SQL Server
                    case enuDatabaseTypeType.SQLServer:
                        blnResult = EditTeamInDatabase2(udtTeam);
                        break;
                }
            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }

            return blnResult;
        }



        // --------------------------------------------------------------------------------
        // Name: EditTeamInDatabase1
        // Abstract: Edit the team in the database
        // --------------------------------------------------------------------------------
        private static bool EditTeamInDatabase1(udtTeamType udtTeam)
        {
            bool blnResult = false;

            try
            {
                string strUpdate = "";
                OleDbCommand cmdUpdate = null;
                int intRowsAffected = 0;

                // Build the UPDATE command. Never build command with raw user input to prevent SQL injections.
                strUpdate = "UPDATE TTeams" +
                            " SET" +
                            " strTeam = ?" +
                            " ,strMascot = ?" +
                            " WHERE" +
                            " intTeamID = ?";

                // Make the command instance
                cmdUpdate = new OleDbCommand(strUpdate, m_conAdministrator);

                // Add column values here instead of above to prevent SQL injection attacks
                cmdUpdate.Parameters.AddWithValue("1", udtTeam.strTeam);
                cmdUpdate.Parameters.AddWithValue("2", udtTeam.strMascot);
                cmdUpdate.Parameters.AddWithValue("3", udtTeam.intTeamID);

                // Insert the row
                intRowsAffected = cmdUpdate.ExecuteNonQuery();

                // Success?
                if (intRowsAffected == 1) blnResult = true;
            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }

            return blnResult;
        }



        // --------------------------------------------------------------------------------
        // Name: EditTeamInDatabase2
        // Abstract: Edit the team in the database
        // --------------------------------------------------------------------------------
        private static bool EditTeamInDatabase2(udtTeamType udtTeam)
        {
            bool blnResult = false;

            try
            {
                OleDbCommand cmdStoredProcedure = null;

                // Create sqlcommand object to run our stored procedure
                cmdStoredProcedure = new OleDbCommand("uspEditTeam", m_conAdministrator);

                cmdStoredProcedure.CommandType = CommandType.StoredProcedure;

                // Add column values here instead of above to prevent SQL injection attacks
                cmdStoredProcedure.Parameters.AddWithValue("1", udtTeam.intTeamID);
                cmdStoredProcedure.Parameters.AddWithValue("2", udtTeam.strTeam);
                cmdStoredProcedure.Parameters.AddWithValue("3", udtTeam.strMascot);

                // Execute the stored procedure
                cmdStoredProcedure.ExecuteNonQuery();

                // Success
                blnResult = true;
            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }

            return blnResult;
        }



        //// --------------------------------------------------------------------------------
        //// Name: DeleteTeamFromDatabase1
        //// Abstract: Delete the team. *** BAD OLD WAY.Massive tech support over accidental deletes***
        //// --------------------------------------------------------------------------------
        //private static bool DeleteTeamFromDatabase1(int intTeamID)
        //{
        //    bool blnResult = false;

        //    try
        //    {
        //        // Delete all the players on the team first
        //        blnResult = DeleteRecordsFromTable(intTeamID, "intTeamID", "TTeamPlayers");

        //        // Was the remove players from team successful?
        //        if(blnResult == true)
        //        {
        //            // Yes
        //            blnResult = DeleteRecordsFromTable(intTeamID, "intTeamID", "TTeams");
        //        }

        //    }
        //    catch(Exception excError)
        //    {
        //        CUtilities.WriteLog(excError);
        //    }

        //    return blnResult;
        //}



        // --------------------------------------------------------------------------------
        // Name: DeleteTeamFromDatabase
        // Abstract: Delete the team from the database
        // --------------------------------------------------------------------------------
        public static bool DeleteTeamFromDatabase(int intTeamID)
        {
            bool blnResult = false;

            try
            {
                //Database type?
                switch (m_enuDatabaseType)
                {
                    //MS Access
                    case enuDatabaseTypeType.MSAccess:
                        blnResult = DeleteTeamFromDatabase1(intTeamID);
                        break;

                    case enuDatabaseTypeType.SQLServer:
                        blnResult = DeleteTeamFromDatabase2(intTeamID);
                        break;
                }

            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }

            return blnResult;
        }



        // --------------------------------------------------------------------------------
        // Name: DeleteTeamFromDatabase1
        // Abstract: Mark the team as inactive
        // --------------------------------------------------------------------------------
        private static bool DeleteTeamFromDatabase1(int intTeamID)
        {
            bool blnResult = false;

            try
            {
                blnResult = true; //SetTeamStatusInDatabase1(intTeamID);
            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }

            return blnResult;
        }



        // --------------------------------------------------------------------------------
        // Name: DeleteTeamFromDatabase2
        // Abstract: Don//t really delete.Just mark as inactive.
        // --------------------------------------------------------------------------------
        private static bool DeleteTeamFromDatabase2(int intTeamID)
        {
            bool blnResult = false;
            try
            {
                blnResult = true; //SetTeamStatusInDatabase2(intTeamID, CConstants.intTEAM_STATUS_INACTIVE);
            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }

            return blnResult;
        }



        // --------------------------------------------------------------------------------
        // Name: UndeleteTeamFromDatabase
        // Abstract: Undelete the team from the database
        // --------------------------------------------------------------------------------
        public static bool UndeleteTeamFromDatabase(int intTeamID)
        {
            bool blnResult = false;

            try
            {
                // Database Type?
                switch (m_enuDatabaseType)
                {
                    case enuDatabaseTypeType.MSAccess:
                        blnResult = UndeleteTeamFromDatabase1(intTeamID);
                        break;
                    case enuDatabaseTypeType.SQLServer:
                        blnResult = UndeleteTeamFromDatabase2(intTeamID);
                        break;
                }
                // MS Access

            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }

            return blnResult;
        }



        // --------------------------------------------------------------------------------
        // Name: UndeleteTeamFromDatabase1
        // Abstract: Lazarus, come out!
        // --------------------------------------------------------------------------------
        private static bool UndeleteTeamFromDatabase1(int intTeamID)
        {
            bool blnResult = false;

            try
            {
                blnResult = true;//SetTeamStatusInDatabase1(intTeamID, CConstants.intTEAM_STATUS_ACTIVE);
            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }

            return blnResult;
        }



        // --------------------------------------------------------------------------------
        // Name: UndeleteTeamFromDatabase2
        // Abstract: Larazus, come out!
        // --------------------------------------------------------------------------------
        private static bool UndeleteTeamFromDatabase2(int intTeamID)
        {
            bool blnResult = false;
            try
            {
                blnResult = true; //SetTeamStatusInDatabase2(intTeamID, CConstants.intTEAM_STATUS_ACTIVE);
            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }

            return blnResult;
        }



        // --------------------------------------------------------------------------------
        // Name: SetTeamStatusInDatabase1
        // Abstract: Mark the specified team as active or inactive
        // --------------------------------------------------------------------------------
        private static bool SetTeamStatusInDatabase1(int intTeamID, int intTeamStatusID)
        {
            bool blnResult = false;

            try
            {
                string strUpdate = "";
                OleDbCommand cmdUpdate = null;
                int intRowsAffected = 0;

                // Build the UPDATE command.
                strUpdate = "UPDATE TTeams" +
                            " SET" +
                            " intTeamStatusID = ?" +
                            " WHERE" +
                            " intTeamID = ?";

                // Make the command instance
                cmdUpdate = new OleDbCommand(strUpdate, m_conAdministrator);

                // Add column values
                cmdUpdate.Parameters.AddWithValue("1", intTeamStatusID);
                cmdUpdate.Parameters.AddWithValue("2", intTeamID);

                // Insert the row
                intRowsAffected = cmdUpdate.ExecuteNonQuery();

                // Success?
                if (intRowsAffected == 1) blnResult = true;
            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }

            return blnResult;
        }



        // --------------------------------------------------------------------------------
        // Name: SetTeamStatusInDatabase2
        // Abstract: Set the status to either active or inactive
        // --------------------------------------------------------------------------------
        private static bool SetTeamStatusInDatabase2(int intTeamID, int intTeamStatusID)
        {
            bool blnResult = false;

            try
            {
                OleDbCommand cmdStoredProcedure = null;

                // Create sqlcommand object to run our stored procedure
                cmdStoredProcedure = new OleDbCommand("uspSetTeamStatus", m_conAdministrator);
                cmdStoredProcedure.CommandType = CommandType.StoredProcedure;

                // Add column values
                cmdStoredProcedure.Parameters.AddWithValue("1", intTeamID);
                cmdStoredProcedure.Parameters.AddWithValue("2", intTeamStatusID);

                // Execute the stored procedure
                cmdStoredProcedure.ExecuteNonQuery();

                // Success
                blnResult = true;
            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }

            return blnResult;
        }

        #endregion

        #region "TPlayers"
        // --------------------------------------------------------------------------------
        // Name: AddPlayerToDatabase
        // Abstract: Add the Player to the database
        // --------------------------------------------------------------------------------
        public static bool AddPlayerToDatabase(ref udtPatientType udtPatient)
        {
            bool blnResult = false;

            try
            {
                // Database type?
                switch (m_enuDatabaseType)
                {
                    // MS Access
                    case enuDatabaseTypeType.MSAccess:
                        blnResult = AddPlayerToDatabase1(udtPatient);
                        break;
                    case enuDatabaseTypeType.SQLServer:
                        blnResult = AddPlayerToDatabase2(ref udtPatient);
                        break;

                }
            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }

            return blnResult;
        }



        // --------------------------------------------------------------------------------
        // Name: AddPlayerToDatabase1
        // Abstract: Add the Player to the database
        // --------------------------------------------------------------------------------
        private static bool AddPlayerToDatabase1(udtPatientType udtPatient)
        {
            bool blnResult = false;

            try
            {
                string strInsert = "";
                OleDbCommand cmdInsert = null;

                // Get the next highest Player ID
                // Race condition. Need an atomic action but not possible in access.

                if (GetNextHighestRecordID("intPatientID", "TPlayers", ref udtPatient.intPatientID) == true)
                {
                    // Build the INSERT command. Never build command with raw user input to prevent SQL injections.
                    strInsert = "INSERT INTO TPlayers ( intPatientID, strFirstName, strLastName, intPlayerStatusID )" +
                                " VALUES ( ?, ?, ?, ? )";

                    // Make the command instance
                    cmdInsert = new OleDbCommand(strInsert, m_conAdministrator);

                    // Add column values here instead of above to prevent SQL injection attacks
                    cmdInsert.Parameters.AddWithValue("1", udtPatient.intPatientID);
                    cmdInsert.Parameters.AddWithValue("2", udtPatient.strFirstName);
                    cmdInsert.Parameters.AddWithValue("3", udtPatient.strLastName);

                    // Insert the row
                    cmdInsert.ExecuteNonQuery();

                    // Success
                    blnResult = true;
                }
            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }

            return blnResult;

        }




        // --------------------------------------------------------------------------------
        // Name: AddPlayerToDatabase2
        // Abstract: How to add a record using a stored procedure that returns
        // the record ID. Use this for SQL Server.
        // Advantages:
        // 1) There is only one back and forth from the code to the database.
        // 2) Using a stored procedure takes much of the SQL out of our
        // code and puts it in the database.
        // 3) Stored procedures are guaranteed to be syntactically correct
        // once created (unless you are doing dynamic queries)
        // 4) Stored procedures are pre-compiled (after first run and then cached)
        // so they execute as quickly as possible.
        // 5) Once started the stored procedure is guaranteed to finish
        // without any further input which is good because we will never
        // have an uncommited transaction.
        // --------------------------------------------------------------------------------
        private static bool AddPlayerToDatabase2(ref udtPatientType udtPatient)
        {
            bool blnResult = false;

            try
            {
                OleDbCommand cmdStoredProcedure = null;
                OleDbDataReader drReturnValues = null;

                if (GetNextHighestRecordID("intPatientID", "TPlayers", ref udtPatient.intPatientID) == true)
                {
                    // Create sqlcommand object to run our stored procedure
                    cmdStoredProcedure = new OleDbCommand("uspAddPlayer", m_conAdministrator);
                    cmdStoredProcedure.CommandType = CommandType.StoredProcedure;

                    // Add parameters
                    cmdStoredProcedure.Parameters.Add(new OleDbParameter("@strFirstName", udtPatient.strFirstName));
                    cmdStoredProcedure.Parameters.Add(new OleDbParameter("@strMiddleName", udtPatient.strMiddleName));
                    cmdStoredProcedure.Parameters.Add(new OleDbParameter("@strLastName", udtPatient.strLastName));
                    cmdStoredProcedure.Parameters.Add(new OleDbParameter("@strHomeAddress", udtPatient.strStreetAddress));
                    cmdStoredProcedure.Parameters.Add(new OleDbParameter("@strCity", udtPatient.strCity));
                    cmdStoredProcedure.Parameters.Add(new OleDbParameter("@intStateID", udtPatient.intStateID));
                    cmdStoredProcedure.Parameters.Add(new OleDbParameter("@strZipCode", udtPatient.strZipCode));
                    cmdStoredProcedure.Parameters.Add(new OleDbParameter("@strPrimaryPhoneNumber", udtPatient.strPrimaryPhoneNumber));
                    //cmdStoredProcedure.Parameters.Add(new OleDbParameter("@decSalary", udtPatient.decSalary));
                    cmdStoredProcedure.Parameters.Add(new OleDbParameter("@dtmDateOfBirth", udtPatient.dtmDateOfBirth));
                    cmdStoredProcedure.Parameters.Add(new OleDbParameter("@intSexID", udtPatient.intSexID));
                   // cmdStoredProcedure.Parameters.Add(new OleDbParameter("@blnMostValuablePlayer", udtPatient.blnMostValuablePlayer));
                    cmdStoredProcedure.Parameters.Add(new OleDbParameter("@strEmailAddress", udtPatient.strEmailAddress));

                    //cmdStoredProcedure.Parameters.AddWithValue("1", udtPatient.strFirstName);
                    //cmdStoredProcedure.Parameters.AddWithValue("2", udtPatient.strMiddleName);
                    //cmdStoredProcedure.Parameters.AddWithValue("3", udtPatient.strLastName);
                    //cmdStoredProcedure.Parameters.AddWithValue("4", udtPatient.strHomeAddress);
                    //cmdStoredProcedure.Parameters.AddWithValue("5", udtPatient.strCity);
                    //cmdStoredProcedure.Parameters.AddWithValue("6", udtPatient.intStateID);
                    //cmdStoredProcedure.Parameters.AddWithValue("7", udtPatient.strZipCode);
                    //cmdStoredProcedure.Parameters.AddWithValue("8", udtPatient.strPrimaryPhoneNumber);
                    //cmdStoredProcedure.Parameters.AddWithValue("9", udtPatient.decSalary);
                    //cmdStoredProcedure.Parameters.AddWithValue("10", udtPatient.dtmDateOfBirth);
                    //cmdStoredProcedure.Parameters.AddWithValue("11", udtPatient.intSexID);
                    //cmdStoredProcedure.Parameters.AddWithValue("12", udtPatient.blnMostValuablePlayer);
                    //cmdStoredProcedure.Parameters.AddWithValue("13", udtPatient.strEmailAddress);

                    // Execute the stored procedure
                    drReturnValues = cmdStoredProcedure.ExecuteReader();

                    // Should be 1 and only 1 record returned
                    drReturnValues.Read();

                    // Get the new ID (could also use an output parameter)
                    udtPatient.intPatientID = (int)Conversion.Val(drReturnValues["intPatientID"].ToString());

                    // Clean up
                    drReturnValues.Close();

                    // Success
                    blnResult = true;
                }
            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }

            return blnResult;
        }



        // --------------------------------------------------------------------------------
        // Name: GetPlayerInformationFromDatabase
        // Abstract: Get data for the specified Player from the database
        // --------------------------------------------------------------------------------
        public static bool GetPlayerInformationFromDatabase(ref udtPatientType udtPatient)
        {
            bool blnResult = false;
            try
            {
                string strSelect = "";
                OleDbCommand cmdSelect = null;
                OleDbDataReader drTPlayers = null;

                // Build the select string
                strSelect = "SELECT *" +
                            " FROM TPlayers" +
                            " WHERE intPatientID = " + udtPatient.intPatientID;

                // Retrieve the record
                cmdSelect = new OleDbCommand(strSelect, m_conAdministrator);

                drTPlayers = cmdSelect.ExecuteReader();

                // Read (there should be 1 and only 1 row)
                drTPlayers.Read();

                udtPatient.strFirstName = drTPlayers["strFirstName"].ToString();
                udtPatient.strMiddleName = drTPlayers["strMiddleName"].ToString();
                udtPatient.strLastName = drTPlayers["strLastName"].ToString();
                udtPatient.strStreetAddress = drTPlayers["strHomeAddress"].ToString();
                udtPatient.strCity = drTPlayers["strCity"].ToString();
                udtPatient.intStateID = (int)drTPlayers["intStateID"];
                udtPatient.strZipCode = drTPlayers["strZipCode"].ToString();
                udtPatient.strPrimaryPhoneNumber = drTPlayers["strPrimaryPhoneNumber"].ToString();
                //udtPatient.decSalary = (decimal)drTPlayers["curSalary"];
                udtPatient.dtmDateOfBirth = (DateTime)drTPlayers["dtmDateOfBirth"];
                udtPatient.intSexID = (int)drTPlayers["intSexID"];
                //udtPatient.blnMostValuablePlayer = (bool)drTPlayers["blnMostValuablePlayer"];
                udtPatient.strEmailAddress = drTPlayers["strEmailAddress"].ToString();

                // Clean up
                drTPlayers.Close();

                // Success
                blnResult = true;
            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }

            return blnResult;
        }



        //// --------------------------------------------------------------------------------
        //// Name: SafeGetString
        //// Abstract: Safely handle null strings.
        //// https://stackoverflow.com/questions/1772025/sql-data-reader-handling-null-column-values
        //// --------------------------------------------------------------------------------
        //public staticstring SafeGetString(OleDbDataReader drTPlayers, string strColumn)
        //{
        //    // Default to empty string, to avoid blowing up.
        //    string strValue = string.Empty;

        //    // Check if it is a null Value
        //    if (drTPlayers[strColumn].ToString() != "")
        //    {
        //        // No, then add it.
        //        strValue = drTPlayers[strColumn].ToString();
        //    }

        //    return strValue;
        //}



        // --------------------------------------------------------------------------------
        // Name: EditPlayerInDatabase
        // Abstract: Edit the Player in the database
        // --------------------------------------------------------------------------------
        public static bool EditPlayerInDatabase(ref udtPatientType udtPatient)
        {
            bool blnResult = false;

            try
            {
                // Database type?
                switch (m_enuDatabaseType)
                {
                    // MS Access
                    case enuDatabaseTypeType.MSAccess:
                        blnResult = EditPlayerInDatabase1(udtPatient);
                        break;
                    // SQL Server
                    case enuDatabaseTypeType.SQLServer:
                        blnResult = EditPlayerInDatabase2(udtPatient);
                        break;
                }
            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }

            return blnResult;
        }



        // --------------------------------------------------------------------------------
        // Name: EditPlayerInDatabase1
        // Abstract: Edit the Player in the database
        // --------------------------------------------------------------------------------
        private static bool EditPlayerInDatabase1(udtPatientType udtPatient)
        {
            bool blnResult = false;

            try
            {
                string strUpdate = "";
                OleDbCommand cmdUpdate = null;
                int intRowsAffected = 0;

                // Build the UPDATE command. Never build command with raw user input to prevent SQL injections.
                strUpdate = "UPDATE TPlayers" +
                            " SET" +
                            " strFirstName = ?" +
                            " ,strLastName = ?" +
                            " WHERE" +
                            " intPatientID = ?";

                // Make the command instance
                cmdUpdate = new OleDbCommand(strUpdate, m_conAdministrator);

                // Add column values here instead of above to prevent SQL injection attacks
                cmdUpdate.Parameters.AddWithValue("1", udtPatient.strFirstName);
                cmdUpdate.Parameters.AddWithValue("2", udtPatient.strLastName);
                cmdUpdate.Parameters.AddWithValue("3", udtPatient.intPatientID);

                // Insert the row
                intRowsAffected = cmdUpdate.ExecuteNonQuery();

                // Success?
                if (intRowsAffected == 1) blnResult = true;
            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }

            return blnResult;
        }



        // --------------------------------------------------------------------------------
        // Name: EditPlayerInDatabase2
        // Abstract: Edit the Player in the database
        // --------------------------------------------------------------------------------
        private static bool EditPlayerInDatabase2(udtPatientType udtPatient)
        {
            bool blnResult = false;

            try
            {
                OleDbCommand cmdStoredProcedure = null;

                // Make the command instance
                cmdStoredProcedure = new OleDbCommand("uspEditPlayer", m_conAdministrator);
                cmdStoredProcedure.CommandType = CommandType.StoredProcedure;

                // Add column values here instead of above to prevent SQL injection attacks
                cmdStoredProcedure.Parameters.Add(new OleDbParameter("@intPatientID", udtPatient.intPatientID));
                cmdStoredProcedure.Parameters.Add(new OleDbParameter("@strFirstName", udtPatient.strFirstName));
                cmdStoredProcedure.Parameters.Add(new OleDbParameter("@strMiddleName", udtPatient.strMiddleName));
                cmdStoredProcedure.Parameters.Add(new OleDbParameter("@strLastName", udtPatient.strLastName));
                cmdStoredProcedure.Parameters.Add(new OleDbParameter("@strHomeAddress", udtPatient.strStreetAddress));
                cmdStoredProcedure.Parameters.Add(new OleDbParameter("@strCity", udtPatient.strCity));
                cmdStoredProcedure.Parameters.Add(new OleDbParameter("@intStateID", udtPatient.intStateID));
                cmdStoredProcedure.Parameters.Add(new OleDbParameter("@strZipCode", udtPatient.strZipCode));
                cmdStoredProcedure.Parameters.Add(new OleDbParameter("@strPrimaryPhoneNumber", udtPatient.strPrimaryPhoneNumber));
                //cmdStoredProcedure.Parameters.Add(new OleDbParameter("@decSalary", udtPatient.decSalary));
                cmdStoredProcedure.Parameters.Add(new OleDbParameter("@dtmDateOfBirth", udtPatient.dtmDateOfBirth));
                cmdStoredProcedure.Parameters.Add(new OleDbParameter("@intSexID", udtPatient.intSexID));
                //cmdStoredProcedure.Parameters.Add(new OleDbParameter("@blnMostValuablePlayer", udtPatient.blnMostValuablePlayer));
                cmdStoredProcedure.Parameters.Add(new OleDbParameter("@strEmailAddress", udtPatient.strEmailAddress));

                //cmdStoredProcedure.Parameters.AddWithValue( "1", udtPatient.intPatientID);
                //cmdStoredProcedure.Parameters.AddWithValue( "2", udtPatient.strFirstName);
                //cmdStoredProcedure.Parameters.AddWithValue( "3", udtPatient.strMiddleName);
                //cmdStoredProcedure.Parameters.AddWithValue( "4", udtPatient.strLastName);
                //cmdStoredProcedure.Parameters.AddWithValue( "5", udtPatient.strHomeAddress);
                //cmdStoredProcedure.Parameters.AddWithValue( "6", udtPatient.strCity);
                //cmdStoredProcedure.Parameters.AddWithValue( "7", udtPatient.intStateID);
                //cmdStoredProcedure.Parameters.AddWithValue( "8", udtPatient.strZipCode);
                //cmdStoredProcedure.Parameters.AddWithValue( "9", udtPatient.strPrimaryPhoneNumber);
                //cmdStoredProcedure.Parameters.AddWithValue("10", udtPatient.decSalary);
                //cmdStoredProcedure.Parameters.AddWithValue("11", udtPatient.dtmDateOfBirth);
                //cmdStoredProcedure.Parameters.AddWithValue("12", udtPatient.intSexID);
                //cmdStoredProcedure.Parameters.AddWithValue("13", udtPatient.blnMostValuablePlayer);
                //cmdStoredProcedure.Parameters.AddWithValue("14", udtPatient.strEmailAddress);

                // Execute the stored procedure
                cmdStoredProcedure.ExecuteReader();

                blnResult = true;
            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }

            return blnResult;
        }



        //// --------------------------------------------------------------------------------
        //// Name: DeletePlayerFromDatabase1
        //// Abstract: Delete the Player. *** BAD OLD WAY.Massive tech support over accidental deletes***
        //// --------------------------------------------------------------------------------
        //private static bool DeletePlayerFromDatabase1(int intPatientID)
        //{
        //    bool blnResult = false;

        //    try
        //    {
        //        // Delete all the players on the Player first
        //        blnResult = DeleteRecordsFromTable(intPatientID, "intPatientID", "TPlayerPlayers");

        //        // Was the remove players from Player successful?
        //        if(blnResult == true)
        //        {
        //            // Yes
        //            blnResult = DeleteRecordsFromTable(intPatientID, "intPatientID", "TPlayers");
        //        }

        //    }
        //    catch(Exception excError)
        //    {
        //        CUtilities.WriteLog(excError);
        //    }

        //    return blnResult;
        //}



        // --------------------------------------------------------------------------------
        // Name: DeletePlayerFromDatabase
        // Abstract: Delete the Player from the database
        // --------------------------------------------------------------------------------
        public static bool DeletePlayerFromDatabase(int intPatientID)
        {
            bool blnResult = false;

            try
            {
                //Database type?
                switch (m_enuDatabaseType)
                {
                    //MS Access
                    case enuDatabaseTypeType.MSAccess:
                        blnResult = DeletePlayerFromDatabase1(intPatientID);
                        break;

                    case enuDatabaseTypeType.SQLServer:
                        blnResult = DeletePlayerFromDatabase2(intPatientID);
                        break;
                }

            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }

            return blnResult;
        }



        // --------------------------------------------------------------------------------
        // Name: DeletePlayerFromDatabase1
        // Abstract: Mark the Player as inactive
        // --------------------------------------------------------------------------------
        private static bool DeletePlayerFromDatabase1(int intPatientID)
        {
            bool blnResult = false;

            try
            {
                blnResult = true; // SetPlayerStatusInDatabase1(intPatientID, CConstants.intPLAYER_STATUS_INACTIVE);
            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }

            return blnResult;
        }



        // --------------------------------------------------------------------------------
        // Name: DeletePlayerFromDatabase2
        // Abstract: Don//t really delete.Just mark as inactive.
        // --------------------------------------------------------------------------------
        private static bool DeletePlayerFromDatabase2(int intPatientID)
        {
            bool blnResult = false;
            try
            {
                blnResult = true; // SetPlayerStatusInDatabase2(intPatientID, CConstants.intPLAYER_STATUS_INACTIVE);
            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }

            return blnResult;
        }



        // --------------------------------------------------------------------------------
        // Name: UndeletePlayerFromDatabase
        // Abstract: Undelete the Player from the database
        // --------------------------------------------------------------------------------
        public static bool UndeletePlayerFromDatabase(int intPatientID)
        {
            bool blnResult = false;

            try
            {
                // Database Type?
                switch (m_enuDatabaseType)
                {
                    case enuDatabaseTypeType.MSAccess:
                        blnResult = UndeletePlayerFromDatabase1(intPatientID);
                        break;
                    case enuDatabaseTypeType.SQLServer:
                        blnResult = UndeletePlayerFromDatabase2(intPatientID);
                        break;
                }
                // MS Access

            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }

            return blnResult;
        }



        // --------------------------------------------------------------------------------
        // Name: UndeletePlayerFromDatabase1
        // Abstract: Lazarus, come out!
        // --------------------------------------------------------------------------------
        private static bool UndeletePlayerFromDatabase1(int intPatientID)
        {
            bool blnResult = false;

            try
            {
                blnResult = true; // SetPlayerStatusInDatabase1(intPatientID, CConstants.intPLAYER_STATUS_ACTIVE);
            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }

            return blnResult;
        }



        // --------------------------------------------------------------------------------
        // Name: UndeletePlayerFromDatabase2
        // Abstract: Larazus, come out!
        // --------------------------------------------------------------------------------
        private static bool UndeletePlayerFromDatabase2(int intPatientID)
        {
            bool blnResult = false;
            try
            {
                blnResult = true; // SetPlayerStatusInDatabase2(intPatientID, CConstants.intPLAYER_STATUS_ACTIVE);
            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }

            return blnResult;
        }



        // --------------------------------------------------------------------------------
        // Name: SetPlayerStatusInDatabase1
        // Abstract: Mark the specified Player as active or inactive
        // --------------------------------------------------------------------------------
        private static bool SetPlayerStatusInDatabase1(int intPatientID, int intPlayerStatusID)
        {
            bool blnResult = false;

            try
            {
                string strUpdate = "";
                OleDbCommand cmdUpdate = null;
                int intRowsAffected = 0;

                // Build the UPDATE command.
                strUpdate = "UPDATE TPlayers" +
                            " SET" +
                            " intPlayerStatusID = ?" +
                            " WHERE" +
                            " intPatientID = ?";

                // Make the command instance
                cmdUpdate = new OleDbCommand(strUpdate, m_conAdministrator);

                // Add column values
                cmdUpdate.Parameters.AddWithValue("1", intPlayerStatusID);
                cmdUpdate.Parameters.AddWithValue("2", intPatientID);

                // Insert the row
                intRowsAffected = cmdUpdate.ExecuteNonQuery();

                // Success?
                if (intRowsAffected == 1) blnResult = true;
            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }

            return blnResult;
        }



        // --------------------------------------------------------------------------------
        // Name: SetPlayerStatusInDatabase2
        // Abstract: Set the status to either active or inactive
        // --------------------------------------------------------------------------------
        private static bool SetPlayerStatusInDatabase2(int intPatientID, int intPlayerStatusID)
        {
            bool blnResult = false;

            try
            {
                OleDbCommand cmdStoredProcedure = null;

                // Create sqlcommand object to run our stored procedure
                cmdStoredProcedure = new OleDbCommand("uspSetPlayerStatus", m_conAdministrator);
                cmdStoredProcedure.CommandType = CommandType.StoredProcedure;

                // Add column values
                cmdStoredProcedure.Parameters.Add(new OleDbParameter("@intPatientID", intPatientID));
                cmdStoredProcedure.Parameters.Add(new OleDbParameter("@intPlayerStatusID", intPlayerStatusID));

                //cmdStoredProcedure.Parameters.AddWithValue("1", intPatientID);
                //cmdStoredProcedure.Parameters.AddWithValue("2", intPlayerStatusID);

                // Execute the stored procedure
                cmdStoredProcedure.ExecuteNonQuery();

                // Success
                blnResult = true;
            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }

            return blnResult;
        }


        #endregion

        #region "TeamsAndPlayers"

        // --------------------------------------------------------------------------------
        // Name: LoadListWithPlayersFromDatabase
        // Abstract: Load all the players on/not on the specified team
        // --------------------------------------------------------------------------------
        public static bool LoadListWithPlayersFromDatabase(int intTeamID, ref ListBox lstTarget, bool blnPlayersOnTeam)
        {
            bool blnResult = false;

            try
            {
                string strCustomSQL = string.Empty;
                string strNot = "NOT";

                // Selected Players
                if (blnPlayersOnTeam == true) strNot = "";

                // Build the Custom SQL, Load all the players that are/are not already on the team
                strCustomSQL = "SELECT " +
                           "    intPatientID, strLastName + ', ' + strFirstName " +
                           " FROM " +
                           "    VActivePlayers " +
                           " WHERE intPatientID " + strNot + " IN " +
                           "    ( " +
                           "      SELECT intPatientID " +
                           "      FROM TTeamPlayers " +
                           "      WHERE intTeamID = " + intTeamID +
                           "    ) " +
                           " ORDER BY " +
                           "    strLastName, strFirstName";

                blnResult = LoadListBoxFromDatabase("", "", "", ref lstTarget, "", strCustomSQL);
            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }

            return blnResult;
        }



        // --------------------------------------------------------------------------------
        // Name: AddPlayerToDatabase
        // Abstract: Add the Player to the database
        // --------------------------------------------------------------------------------
        public static bool AddPlayerToTeamInDatabase(int intPatientID, int intTeamID)
        {
            bool blnResult = false;

            try
            {
                // Database type?
                switch (m_enuDatabaseType)
                {
                    // MS Access
                    case enuDatabaseTypeType.MSAccess:
                        blnResult = AddPlayerToTeamInDatabase1(intPatientID, intTeamID);
                        break;
                    case enuDatabaseTypeType.SQLServer:
                        blnResult = AddPlayerToTeamInDatabase2(intPatientID, intTeamID);
                        break;

                }
            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }

            return blnResult;
        }



        // --------------------------------------------------------------------------------
        // Name: AddPlayerToTeamInDatabase1
        // Abstract: Add a Player to a Team in Access
        // --------------------------------------------------------------------------------
        private static bool AddPlayerToTeamInDatabase1(int intTeamID, int intPatientID)
        {
            bool blnResult = false;

            try
            {
                OleDbCommand cmdInsert = null;
                string strInsert = string.Empty;

                // Build the INSERT command. Never build command with raw user input to prevent SQL injections.
                strInsert = "INSERT INTO TTeamPlayers ( intTeamID, , intPatientID )" +
                            " VALUES ( ?, ? )";

                // Create sqlcommand object to run our stored procedure
                cmdInsert = new OleDbCommand(strInsert, m_conAdministrator);

                // Add parameters
                cmdInsert.Parameters.AddWithValue("1", intTeamID);
                cmdInsert.Parameters.AddWithValue("2", intPatientID);

                // Execute the stored procedure
                cmdInsert.ExecuteNonQuery();

                // Success
                blnResult = true;
            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }

            return blnResult;
        }



        // --------------------------------------------------------------------------------
        // Name: AddPlayerToTeamInDatabase2
        // Abstract: Add a Player to a Team in SQL
        // --------------------------------------------------------------------------------
        private static bool AddPlayerToTeamInDatabase2(int intTeamID, int intPatientID)
        {
            bool blnResult = false;

            try
            {
                OleDbCommand cmdStoredProcedure = null;

                // Create sqlcommand object to run our stored procedure
                cmdStoredProcedure = new OleDbCommand("uspAddTeamPlayer", m_conAdministrator);
                cmdStoredProcedure.CommandType = CommandType.StoredProcedure;

                // Add parameters
                cmdStoredProcedure.Parameters.AddWithValue("1", intTeamID);
                cmdStoredProcedure.Parameters.AddWithValue("2", intPatientID);

                // Execute the stored procedure
                cmdStoredProcedure.ExecuteNonQuery();

                // Success
                blnResult = true;
            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }

            return blnResult;
        }



        // --------------------------------------------------------------------------------
        // Name: AddPlayerToDatabase
        // Abstract: Add the Player to the database
        // --------------------------------------------------------------------------------
        public static bool RemovePlayerFromTeamInDatabase(int intPatientID, int intTeamID)
        {
            bool blnResult = false;

            try
            {
                // Database type?
                switch (m_enuDatabaseType)
                {
                    // MS Access
                    case enuDatabaseTypeType.MSAccess:
                        blnResult = RemovePlayerFromTeamInDatabase1(intPatientID, intTeamID);
                        break;
                    case enuDatabaseTypeType.SQLServer:
                        blnResult = RemovePlayerFromTeamInDatabase2(intPatientID, intTeamID);
                        break;

                }
            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }

            return blnResult;
        }



        // --------------------------------------------------------------------------------
        // Name: AddPlayerToDatabase
        // Abstract: Add the Player to the database
        // --------------------------------------------------------------------------------
        public static bool RemovePlayerFromTeamInDatabase1(int intPatientID, int intTeamID)
        {
            bool blnResult = false;

            try
            {
                string strCustomSQL = string.Empty;

                strCustomSQL = "DELETE FROM TTeamPlayers" +
                               " WHERE intTeamID   = " + intTeamID +
                               " AND   intPatientID = " + intPatientID;

                blnResult = DeleteRecordsFromTable(0, "", "", strCustomSQL);
            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }

            return blnResult;
        }



        // --------------------------------------------------------------------------------
        // Name: RemovePlayerFromTeamInDatabase2
        // Abstract: 
        // --------------------------------------------------------------------------------
        public static bool RemovePlayerFromTeamInDatabase2(int intTeamID, int intPatientID)
        {
            bool blnResult = false;

            try
            {
                OleDbCommand cmdStoredProcedure = null;



                // Create SQL Command object to run our stored procedure
                cmdStoredProcedure = new OleDbCommand("uspRemoveTeamPlayer", m_conAdministrator);
                cmdStoredProcedure.CommandType = CommandType.StoredProcedure;

                // Add parameters
                cmdStoredProcedure.Parameters.Add(new OleDbParameter("@intTeamID", intTeamID));
                cmdStoredProcedure.Parameters.Add(new OleDbParameter("@intPatientID", intPatientID));

                // Execute stored procedure
                cmdStoredProcedure.ExecuteNonQuery();

                // Did it work?
                blnResult = true;
            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }

            return blnResult;
        }

        #endregion
    }
}