// --------------------------------------------------------------------------------
// Name: CUserDataTypes
// Abstract: 
// --------------------------------------------------------------------------------


// --------------------------------------------------------------------------------
// Imports
// --------------------------------------------------------------------------------
using System;
namespace nsImportAndExportAllInOne
{
    // Add readability by associating text with each numeric value
    public enum enuDatabaseTypeType
    {
        MSAccess = 1,
        SQLServer = 2,
    }

    public enum enuEmailComplexity
    {
        EASY = 1,
        COMPLEX = 2,
    }

    // Teams
    public class udtTeamType
    {
        public int intTeamID = 0;
        public string strTeam = string.Empty;
        public string strMascot = string.Empty;
    }

    #region IMPORT
    // Patients
    public class udtPatientType
    {
        public int intPatientID = 0;
        public string strFirstName = string.Empty;
        public string strMiddleName = string.Empty;
        public string strLastName = string.Empty;
        public string strStreetAddress = string.Empty;
        public string strCity = string.Empty;
        public int intStateID = 0;
        public string strZipCode = string.Empty;
        public string strPrimaryPhoneNumber = string.Empty;
        public string strSecondaryPhoneNumber = string.Empty;
        public bool blnSmoker;
        public decimal decPackYears = 0;
        public bool blnHeadOfHouseHold;
        public DateTime dtmDateOfBirth;
        public int intRecordCreatorID = 0;
        public int intSexID = 0;
        public string strEmailAddress = string.Empty;
    }


    // Record Creator
    public class udtRecordCreatorType
    {
        public int intRecordCreatorID = 0;
        public string strRecordCreator = string.Empty;
    }


    // Medication
    public class udtMedicationType
    {
        public int intMedicationID = 0;
        public string strMedication = string.Empty;
    }


    // Allergy
    public class udtAllergyType
    {
        public int intAllergyID = 0;
        public string strAllergy = string.Empty;
    }


    // Condition
    public class udtConditionType
    {
        public int intConditionID = 0;
        public string strCondition = string.Empty;
    }


    // Procedure
    public class udtProcedureType
    {
        public int intProcedureID = 0;
        public string strProcedure = string.Empty;
    }
    #endregion

    #region EXPORT
    public class udtExportType
    {
        public int intPatientID = 0;
        public string strFirstName = string.Empty;
        public string strMiddleName = string.Empty;
        public string strLastName = string.Empty;
        public string strStreetAddress = string.Empty;
        public string strCity = string.Empty;
        public string strState = string.Empty;
        public string strZipCode = string.Empty;
        public string strPrimaryPhoneNumber = string.Empty;
        public string strSecondaryPhoneNumber = string.Empty;
        public string blnSmoker;
        public string decPackYears = string.Empty;
        public string blnHeadOfHouseHold;
        public string dtmDateOfBirth;
        public string strRecordCreatorID = string.Empty;
        public string strSex = string.Empty;

        public string strAllergy1 = string.Empty;
        public string strAllergy2 = string.Empty;
        public string strAllergy1Medication1 = string.Empty;
        public string strAllergy1Medication2 = string.Empty;
        public string strAllergy2Medication1 = string.Empty;
        public string strAllergy2Medication2 = string.Empty;

        public string strCondition1 = string.Empty;
        public string strCondition2 = string.Empty;
        public string strCondition1Medication1 = string.Empty;
        public string strCondition1Medication2 = string.Empty;
        public string strCondition2Medication1 = string.Empty;
        public string strCondition2Medication2 = string.Empty;

        public string strProcedure1 = string.Empty;
        public string strProcedure2 = string.Empty;
        public string strProcedure3 = string.Empty;

        public string strEmailAddress = string.Empty;
        public string strRecordCreator = string.Empty;


    }

    #endregion
}

