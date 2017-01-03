using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nsImportAndExportAllInOne
{
    class CConstants
    {
        // String Locations
        // --------
        public const int FULL_NAME_START_LOCATION                   = 0;
        public const int STREET_ADDRESS_LOCATION                    = 30;
        public const int CITY_LOCATION                              = 50;
        public const int STATE_LOCATION                             = 70;
        public const int ZIP_LOCATION                               = 72;
        public const int PRIMARY_PHONE_LOCATION                     = 81;
        public const int SECONDARY_PHONE_LOCATION                   = 91;
        public const int SMOKER_LOCATION                            = 106;          // SMOKER_STATUS_LOCATION
        public const int PACK_YEARS_LOCATION                        = 107;
        public const int HEAD_OF_HOUSEHOLD_LOCATION                 = 110;
        public const int DATE_OF_BIRTH_LOCATION                     = 111;
        public const int SEX_LOCATION                               = 122;
        public const int ALLERGY_1_LOCATION                         = 123;
        public const int ALLERGY_2_LOCATION                         = 139;
        public const int ALLERGY_1_MEDICATION_1_LOCATION            = 154;
        public const int ALLERGY_1_MEDICATION_2_LOCATION            = 169;
        public const int ALLERGY_2_MEDICATION_1_LOCATION            = 184;
        public const int ALLERGY_2_MEDICATION_2_LOCATION            = 199;
        // --------




        public const int CONDITION_1_LOCATION                       = 214;
        public const int CONDITION_2_LOCATION                       = 229;
        public const int CONDITION_1_MEDICATION_1_LOCATION          = 244;
        public const int CONDITION_1_MEDICATION_2_LOCATION          = 259;
        public const int CONDITION_2_MEDICATION_1_LOCATION          = 274;
        public const int CONDITION_2_MEDICATION_2_LOCATION          = 289;
        public const int PROCEDURE_1_LOCATION                       = 304;
        public const int PROCEDURE_2_LOCATION                       = 319;
        public const int PROCEDURE_3_LOCATION                       = 334;
        



        // --------
        public const int EMAIL_ADDRESS_LOCATION                     = 349;
        public const int RECORD_CREATED_BY_LOCATION                 = 379;
        // --------

        // String Lengths
        public const int FULL_NAME_LENGTH                           = 30;
        public const int STREET_ADDRESS_LENGTH                      = 20;
        public const int CITY_LENGTH                                = 20;
        public const int STATE_LENGTH                               = 2;
        public const int ZIP_LENGTH                                 = 9;
        public const int PRIMARY_PHONE_LENGTH                       = 10;
        public const int SECONDARY_PHONE_LENGTH                     = 15;
        public const int SMOKER_LENGTH                              = 1;
        public const int PACK_YEARS_LENGTH                          = 3;
        public const int HEAD_OF_HOUSEHOLD_LENGTH                   = 1;
        public const int DATE_OF_BIRTH_LENGTH                       = 11;
        public const int SEX_LENGTH                                 = 1;
        public const int ALLERGY_1_LENGTH                           = 16;
        public const int ALLERGY_2_LENGTH                           = 15;
        public const int ALLERGY_1_MEDICATION_1_LENGTH              = 15;
        public const int ALLERGY_1_MEDICATION_2_LENGTH              = 15;
        public const int ALLERGY_2_MEDICATION_1_LENGTH              = 15;
        public const int ALLERGY_2_MEDICATION_2_LENGTH              = 15;
        public const int CONDITION_1_LENGTH                         = 15;
        public const int CONDITION_2_LENGTH                         = 15;
        public const int CONDITION_1_MEDICATION_1_LENGTH            = 15;
        public const int CONDITION_1_MEDICATION_2_LENGTH            = 15;
        public const int CONDITION_2_MEDICATION_1_LENGTH            = 15;
        public const int CONDITION_2_MEDICATION_2_LENGTH            = 15;
        public const int PROCEDURE_1_LENGTH                         = 15;
        public const int PROCEDURE_2_LENGTH                         = 15;
        public const int PROCEDURE_3_LENGTH                         = 15;
        public const int EMAIL_ADDRESS_LENGTH                       = 30;
        public const int RECORD_CREATED_BY_LENGTH                   = 20;
    }
}
