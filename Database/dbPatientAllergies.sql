-- --------------------------------------------------------------------------------
-- Name: Adam W. Schneider
-- Class: C#
-- --------------------------------------------------------------------------------

-- --------------------------------------------------------------------------------
-- Options
-- --------------------------------------------------------------------------------
USE dbPatientAllergies; -- Get out of the master database
SET NOCOUNT ON; -- Report only errors
GO


-- --------------------------------------------------------------------------------
-- Drop Tables
-- --------------------------------------------------------------------------------
-- Tables
IF OBJECT_ID( 'TPatientConditionMedications' )			IS NOT NULL DROP TABLE TPatientConditionMedications
IF OBJECT_ID( 'TPatientAllergyMedications' )			IS NOT NULL DROP TABLE TPatientAllergyMedications
IF OBJECT_ID( 'TPatientAllergies' )						IS NOT NULL DROP TABLE TPatientAllergies
IF OBJECT_ID( 'TPatientConditions' )					IS NOT NULL DROP TABLE TPatientConditions
IF OBJECT_ID( 'TPatientProcedures' )					IS NOT NULL DROP TABLE TPatientProcedures
IF OBJECT_ID( 'TPatients' )								IS NOT NULL DROP TABLE TPatients
IF OBJECT_ID( 'TRecordCreators' )						IS NOT NULL DROP TABLE TRecordCreators
IF OBJECT_ID( 'TSexes' )								IS NOT NULL DROP TABLE TSexes
IF OBJECT_ID( 'TStates' )								IS NOT NULL DROP TABLE TStates
IF OBJECT_ID( 'TAllergies' )							IS NOT NULL DROP TABLE TAllergies
IF OBJECT_ID( 'TConditions' )							IS NOT NULL DROP TABLE TConditions
IF OBJECT_ID( 'TProcedures' )							IS NOT NULL DROP TABLE TProcedures
IF OBJECT_ID( 'TMedications' )							IS NOT NULL DROP TABLE TMedications

-- Views
IF OBJECT_ID( 'VSmokers' )								IS NOT NULL DROP VIEW VSmokers
IF OBJECT_ID( 'VNonSmokers' )							IS NOT NULL DROP VIEW VNonSmokers
IF OBJECT_ID( 'VHeadOfHouseHold' )						IS NOT NULL DROP VIEW VHeadOfHouseHold
IF OBJECT_ID( 'VNotHeadOfHouseHold' )					IS NOT NULL DROP VIEW VNotHeadOfHouseHold

IF OBJECT_ID( 'VPatientsData' )							IS NOT NULL DROP VIEW VPatientsData
IF OBJECT_ID( 'VPatientAllergysData' )					IS NOT NULL DROP VIEW VPatientAllergyData
IF OBJECT_ID( 'VPatientAllergyMedicationsData' )		IS NOT NULL DROP VIEW VPatientAllergyMedicationsData
IF OBJECT_ID( 'VPatientConditionsData' )				IS NOT NULL DROP VIEW VPatientConditionsData
IF OBJECT_ID( 'VPatientConditionMedicationsData' )		IS NOT NULL DROP VIEW VPatientConditionMedicationsData
IF OBJECT_ID( 'VPatientProceduresData' )				IS NOT NULL DROP VIEW VPatientProceduresData

-- User Stored Procedures
IF OBJECT_ID( 'uspAddPatient' )							IS NOT NULL DROP PROCEDURE uspAddPatient
IF OBJECT_ID( 'uspGetOrAddMedication' )					IS NOT NULL DROP PROCEDURE uspGetOrAddMedication
IF OBJECT_ID( 'uspGetOrAddAllergy' )					IS NOT NULL DROP PROCEDURE uspGetOrAddAllergy
IF OBJECT_ID( 'uspGetOrAddProcedure' )					IS NOT NULL DROP PROCEDURE uspGetOrAddProcedure
IF OBJECT_ID( 'uspGetOrAddCondition' )					IS NOT NULL DROP PROCEDURE uspGetOrAddCondition
IF OBJECT_ID( 'uspGetOrAddRecordCreator' )				IS NOT NULL DROP PROCEDURE uspGetOrAddRecordCreator
IF OBJECT_ID( 'uspAddPatientAllergy' )					IS NOT NULL DROP PROCEDURE uspAddPatientAllergy
IF OBJECT_ID( 'uspAddPatientCondition' )				IS NOT NULL DROP PROCEDURE uspAddPatientCondition
IF OBJECT_ID( 'uspAddPatientProcedure' )				IS NOT NULL DROP PROCEDURE uspAddPatientProcedure
IF OBJECT_ID( 'uspAddPatientConditionMedication' )		IS NOT NULL DROP PROCEDURE uspAddPatientConditionMedication
IF OBJECT_ID( 'uspAddPatientAllergyMedication' )		IS NOT NULL DROP PROCEDURE uspAddPatientAllergyMedication



SELECT * FROM VPatientsData						
SELECT * FROM VPatientAllergysData			
SELECT * FROM VPatientAllergyMedicationsData	
SELECT * FROM VPatientConditionsData			
SELECT * FROM VPatientConditionMedicationsData	
SELECT * FROM VPatientProceduresData			





-- ----------------------------------------------------------------------
-- Tables
-- ----------------------------------------------------------------------
CREATE TABLE TPatients
(
	intPatientID                    Integer          NOT NULL,
	strFirstName                    VarChar(50)      NOT NULL,
	strMiddleName                   VarChar(50)      NOT NULL,
	strLastName                     VarChar(50)      NOT NULL,
	strStreetAddress                VarChar(50)      NOT NULL,
	strCity                         VarChar(50)      NOT NULL,
	intStateID                      Integer          NOT NULL,
	strZipcode						VarChar(50)		 NOT NULL,
	strPrimaryPhoneNumber			VarChar(50)		 NOT NULL,
	strSecondaryPhoneNumber			VarChar(50)		 NOT NULL,
	blnSmoker                       Bit              NOT NULL,
	decPackYears					Decimal(38,0)    NOT NULL,
	blnHeadOfHousehold              Bit              NOT NULL,
	dtmDateOfBirth					DateTime		 NOT NULL,
	intRecordCreatorID              Integer          NOT NULL,
	intSexID                        Integer          NOT NULL,
	strEmailAddress					VarChar(50)		 NOT NULL,
	CONSTRAINT TPatients_PK PRIMARY KEY CLUSTERED ( intPatientID )
)
GO

CREATE TABLE TSexes
(
	intSexID                        Integer          NOT NULL,
	strSex                          VarChar(50)      NOT NULL,
	CONSTRAINT TTSexes_PK PRIMARY KEY CLUSTERED ( intSexID )
)
GO

CREATE TABLE TStates
(
	intStateID                      Integer          NOT NULL,
	strState                        VarChar(50)      NOT NULL,
	strStateAbbreviation            VarChar(50)      NOT NULL,
	CONSTRAINT TStates_PK PRIMARY KEY CLUSTERED ( intStateID )
)
GO

CREATE TABLE TRecordCreators
(
	intRecordCreatorID              Integer          NOT NULL,
	strFullName						VarChar(50)		 NOT NULL,
	CONSTRAINT TRecordCreators_PK PRIMARY KEY CLUSTERED ( intRecordCreatorID )
)
GO

CREATE TABLE TAllergies
(
	intAllergyID                    Integer          NOT NULL,
	strAllergy                      VarChar(50)      NOT NULL,
	CONSTRAINT TAllergies_PK PRIMARY KEY CLUSTERED ( intAllergyID )
)
GO

CREATE TABLE TConditions
(
	intConditionID                  Integer          NOT NULL,
	strCondition                    VarChar(50)      NOT NULL,
	CONSTRAINT TConditions_PK PRIMARY KEY CLUSTERED ( intConditionID )
)
GO

CREATE TABLE TProcedures
(
	intProcedureID                  Integer          NOT NULL,
	strProcedure                    VarChar(50)      NOT NULL,
	CONSTRAINT TProcedures_PK PRIMARY KEY CLUSTERED ( intProcedureID )
)
GO

CREATE TABLE TMedications
(
	intMedicationID                 Integer          NOT NULL,
	strMedication                   VarChar(50)      NOT NULL,
	CONSTRAINT TMedications_PK PRIMARY KEY CLUSTERED ( intMedicationID )
)
GO

CREATE TABLE TPatientAllergies
(
	intPatientID                    Integer          NOT NULL,
	intAllergyID                    Integer          NOT NULL,
	CONSTRAINT TPatientAllergies_PK PRIMARY KEY CLUSTERED ( intPatientID, intAllergyID )
)
GO

CREATE TABLE TPatientAllergyMedications
(
	intPatientID                    Integer          NOT NULL,
	intAllergyID                    Integer          NOT NULL,
	intMedicationID                 Integer          NOT NULL,
	CONSTRAINT TPatientAllergyMedications_PK PRIMARY KEY CLUSTERED ( intPatientID, intAllergyID, intMedicationID )
)
GO

CREATE TABLE TPatientConditions
(
	intPatientID                    Integer          NOT NULL,
	intConditionID                  Integer          NOT NULL,
	CONSTRAINT TPatientConditions_PK PRIMARY KEY CLUSTERED ( intPatientID, intConditionID )
)
GO

CREATE TABLE TPatientProcedures
(
	intPatientID                    Integer          NOT NULL,
	intProcedureID                  Integer          NOT NULL,
	CONSTRAINT TPatientProcedures_PK PRIMARY KEY CLUSTERED ( intPatientID, intProcedureID )
)
GO

CREATE TABLE TPatientConditionMedications
(
	intPatientID                    Integer          NOT NULL,
	intConditionID                  Integer          NOT NULL,
	intMedicationID                 Integer          NOT NULL,
	CONSTRAINT TPatientConditionMedications_PK PRIMARY KEY CLUSTERED ( intPatientID, intConditionID, intMedicationID )
)
GO

-- ----------------------------------------------------------------------
-- Check Constraints
-- ----------------------------------------------------------------------

-- ----------------------------------------------------------------------
-- Unique Constraints
-- ----------------------------------------------------------------------

-- ----------------------------------------------------------------------
-- Foreign Keys
-- ----------------------------------------------------------------------
ALTER TABLE TPatientAllergies ADD CONSTRAINT TPatientAllergies_TAllergies_FK1
FOREIGN KEY( intAllergyID ) REFERENCES TAllergies ( intAllergyID )
GO

ALTER TABLE TPatients ADD CONSTRAINT TPatients_TStates_FK1
FOREIGN KEY( intStateID ) REFERENCES TStates ( intStateID )
GO

ALTER TABLE TPatients ADD CONSTRAINT TPatients_TSexes_FK1
FOREIGN KEY( intSexID ) REFERENCES TSexes ( intSexID )
GO

ALTER TABLE TPatients ADD CONSTRAINT TPatients_TRecordCreators_FK1
FOREIGN KEY( intRecordCreatorID ) REFERENCES TRecordCreators ( intRecordCreatorID )
GO

ALTER TABLE TPatientProcedures ADD CONSTRAINT TPatientProcedures_TProcedures_FK1
FOREIGN KEY( intProcedureID ) REFERENCES TProcedures ( intProcedureID )
GO

ALTER TABLE TPatientAllergyMedications ADD CONSTRAINT TPatientAllergyMedications_TMedications_FK1
FOREIGN KEY( intMedicationID ) REFERENCES TMedications ( intMedicationID )
GO

ALTER TABLE TPatientProcedures ADD CONSTRAINT TPatientProcedures_TPatients_FK1
FOREIGN KEY( intPatientID ) REFERENCES TPatients ( intPatientID )
GO

ALTER TABLE TPatientConditions ADD CONSTRAINT TPatientConditions_TPatients_FK1
FOREIGN KEY( intPatientID ) REFERENCES TPatients ( intPatientID )
GO

ALTER TABLE TPatientConditions ADD CONSTRAINT TPatientConditions_TConditions_FK1
FOREIGN KEY( intConditionID ) REFERENCES TConditions ( intConditionID )
GO

ALTER TABLE TPatientAllergies ADD CONSTRAINT TPatientAllergies_TPatients_FK1
FOREIGN KEY( intPatientID ) REFERENCES TPatients ( intPatientID )
GO

ALTER TABLE TPatientAllergyMedications ADD CONSTRAINT TPatientAllergyMedications_TPatients_FK1
FOREIGN KEY( intPatientID ) REFERENCES TPatients ( intPatientID )
GO

ALTER TABLE TPatientAllergyMedications ADD CONSTRAINT TPatientAllergyMedications_TAllergies_FK1
FOREIGN KEY( intAllergyID ) REFERENCES TAllergies ( intAllergyID )
GO

ALTER TABLE TPatientConditionMedications ADD CONSTRAINT TPatientConditionMedications_TPatients_FK1
FOREIGN KEY( intPatientID ) REFERENCES TPatients ( intPatientID )
GO

ALTER TABLE TPatientConditionMedications ADD CONSTRAINT TPatientConditionMedications_TConditions_FK1
FOREIGN KEY( intConditionID ) REFERENCES TConditions ( intConditionID )
GO

ALTER TABLE TPatientConditionMedications ADD CONSTRAINT TPatientConditionMedications_TMedications_FK1
FOREIGN KEY( intMedicationID ) REFERENCES TMedications ( intMedicationID )
GO


-- ----------------------------------------------------------------------
-- Indexes
-- ----------------------------------------------------------------------

-- ----------------------------------------------------------------------
-- Views
-- ----------------------------------------------------------------------
 
 -- --------------------------------------------------------------------------------
 -- IMPORT
 -- --------------------------------------------------------------------------------
	 -- --------------------------------------------------------------------------------
	 -- Name: VSmokers
	 -- Abstract: Shows all the patients that are Smokers
	 -- --------------------------------------------------------------------------------
	GO
	CREATE VIEW VSmokers
	AS
	 SELECT 
		 TP.intPatientID	AS intPatientID
		,TP.strFirstName	AS strTeam
		,TP.strLastName		AS strLastName
	
	 FROM 
		TPatients			AS TP
 
	 WHERE 
		TP.blnSmoker = 1
	GO



	 -- --------------------------------------------------------------------------------
	 -- Name: VNonSmokers
	 -- Abstract: Shows all the patients that are Not Smokers
	 -- --------------------------------------------------------------------------------
	GO
	CREATE VIEW VNonSmokers
	AS
	 SELECT 
		 TP.intPatientID	AS intPatientID
		,TP.strFirstName	AS strTeam
		,TP.strLastName		AS strLastName
	
	 FROM 
		TPatients			AS TP
 
	 WHERE 
		TP.blnSmoker = 0
	GO



	 -- --------------------------------------------------------------------------------
	 -- Name: VHeadOfHouseHold
	 -- Abstract: Shows all the patients that are Head Of Household
	 -- --------------------------------------------------------------------------------
	GO
	CREATE VIEW VHeadOfHouseHold
	AS
	 SELECT 
		 TP.intPatientID	AS intPatientID
		,TP.strFirstName	AS strTeam
		,TP.strLastName		AS strLastName
	
	 FROM 
		TPatients			AS TP
 
	 WHERE 
		TP.blnHeadOfHousehold = 1
	GO



	 -- --------------------------------------------------------------------------------
	 -- Name: VNotHeadOfHouseHold
	 -- Abstract: Shows all the patients that are Not Head Of Household
	 -- --------------------------------------------------------------------------------
	GO
	CREATE VIEW VNotHeadOfHouseHold
	AS
	 SELECT 
		 TP.intPatientID	AS intPatientID
		,TP.strFirstName	AS strTeam
		,TP.strLastName		AS strLastName
 
	 FROM 
		TPatients			AS TP
 
	 WHERE 
		TP.blnHeadOfHousehold = 0
	GO

	 -- --------------------------------------------------------------------------------
	 -- Name: VPatientsData
	 -- Abstract: Shows all the Export patient Data
	 -- --------------------------------------------------------------------------------
	GO
	CREATE VIEW VPatientsData
	AS
		SELECT
			 TP.intPatientID
			,TP.strLastName + ', ' + TP.strFirstName + ' ' + TP.strMiddleName AS strPatientFullName
			,TP.strStreetAddress
			,TP.strCity
			,TS.strState
			,TP.strZipcode
			,TP.strPrimaryPhoneNumber
			,TP.strSecondaryPhoneNumber
			,TP.blnSmoker
			,TP.decPackYears
			,TP.blnHeadOfHousehold
			,TP.dtmDateOfBirth
			,TG.strSex
			,TP.strEmailAddress
			,TR.strFullName			AS strRecordCreator

		FROM 
			TPatients AS TP
				INNER JOIN TStates AS TS
					ON (TP.intStateID = TS.intStateID)
				INNER JOIN TSexes AS TG
					ON (TP.intSexID = TG.intSexID)
				INNER JOIN TRecordCreators AS TR
					ON (TP.intRecordCreatorID = TR.intRecordCreatorID)
	GO



	 -- --------------------------------------------------------------------------------
	 -- Name: VPatientAllergysData
	 -- Abstract: Shows all the patient Allergys
	 -- --------------------------------------------------------------------------------
	GO
	CREATE VIEW VPatientAllergysData
	AS
		SELECT
		 TP.intPatientID
		,TA.strAllergy

		FROM
		TPatients AS TP
			FULL OUTER JOIN TPatientAllergies AS TPA
				INNER JOIN TAllergies AS TA
				ON (TA.intAllergyID = TPA.intAllergyID)
			ON (TPA.intPatientID = TP.intPatientID)
	GO

	select * from VPatientAllergyMedicationsExportData

	-- -- --------------------------------------------------------------------------------
	-- -- Name: VPatientAllergyMedicationsData
	-- -- Abstract: Shows all the patient Allergy Medication Data
	-- -- --------------------------------------------------------------------------------
	--GO
	--CREATE VIEW VPatientAllergyMedicationsExportData 
	--AS
	--	SELECT
	--	 TP.intPatientID,
	--	 TA.strAllergy +';'+
	--	 CAST(TA.intAllergyID AS VARCHAR(50)) +';'+
	--	 TM.strMedication +';' AS PatientAllergyMedsExportData


	--	FROM
	--	TPatients AS TP
	--		FULL OUTER JOIN TPatientAllergyMedications AS TPAM
	--			INNER JOIN TMedications AS TM
	--				ON (TPAM.intMedicationID = TM.intMedicationID)
	--				INNER JOIN TPatientAllergies AS TPA
	--					INNER JOIN TAllergies AS TA
	--					ON (TA.intAllergyID = TPA.intAllergyID)
	--				ON (TPA.intPatientID = TPAM.intPatientID
	--				AND TPA.intAllergyID = TPAM.intAllergyID)
	--			ON (TPAM.intPatientID = TP.intPatientID)
	--GO

	 -- --------------------------------------------------------------------------------
	 -- Name: VPatientAllergyMedicationsData
	 -- Abstract: Shows all the patient Allergy Medication Data
	 -- --------------------------------------------------------------------------------
	GO
	CREATE VIEW VPatientAllergyMedicationsData 
	AS
		SELECT
		 TP.intPatientID
		,TA.intAllergyID
		,TM.strMedication


		FROM
		TPatients AS TP
			FULL OUTER JOIN TPatientAllergyMedications AS TPAM
				INNER JOIN TMedications AS TM
					ON (TPAM.intMedicationID = TM.intMedicationID)
					INNER JOIN TPatientAllergies AS TPA
						INNER JOIN TAllergies AS TA
						ON (TA.intAllergyID = TPA.intAllergyID)
					ON (TPA.intPatientID = TPAM.intPatientID
					AND TPA.intAllergyID = TPAM.intAllergyID)
				ON (TPAM.intPatientID = TP.intPatientID)
	GO


	 -- --------------------------------------------------------------------------------
	 -- Name: VPatientConditions
	 -- Abstract: Shows all the patient Conditions
	 -- --------------------------------------------------------------------------------
	GO
	CREATE VIEW VPatientConditionsData
	AS
		SELECT
		 TP.intPatientID
		,TC.strCondition

		FROM
		TPatients AS TP
			FULL OUTER JOIN TPatientConditions AS TPC
				INNER JOIN TConditions AS TC
				ON (TC.intConditionID = TPC.intConditionID)
			ON (TPC.intPatientID = TP.intPatientID)
	GO




	 -- --------------------------------------------------------------------------------
	 -- Name: VPatientConditionMedicationsData
	 -- Abstract: Shows all the patient Condition Medication Data
	 -- --------------------------------------------------------------------------------
	GO
	CREATE VIEW VPatientConditionMedicationsData
	AS
		-- PatientCondition
		SELECT
			 TP.intPatientID
			,TC.intConditionID
			,TM.strMedication

		FROM
			TPatients AS TP
				FULL OUTER JOIN TPatientConditionMedications AS TPCM
					INNER JOIN TMedications AS TM
						ON (TPCM.intMedicationID = TM.intMedicationID)
					INNER JOIN TPatientConditions AS TPC
						INNER JOIN TConditions AS TC
							ON (TC.intConditionID = TPC.intConditionID)
						ON (TPC.intPatientID = TPCM.intPatientID
						AND TPC.intConditionID = TPCM.intConditionID)
					ON (TPCM.intPatientID = TP.intPatientID)
	GO



	 -- --------------------------------------------------------------------------------
	 -- Name: VPatientProceduresData
	 -- Abstract: Shows all the patient procedures Data
	 -- --------------------------------------------------------------------------------
	GO
	CREATE VIEW VPatientProceduresData
	AS
		-- PatientProcedure
		SELECT
			 TP.intPatientID
			,TPr.strProcedure
		FROM
			TPatients as TP
				FULL OUTER JOIN TPatientProcedures AS TPP
					INNER JOIN TProcedures AS TPr
					ON (TPP.intProcedureID = TPr.intProcedureID)
				ON (TPP.intPatientID = TP.intPatientID)	
	GO	
GO





 -- --------------------------------------------------------------------------------
 -- EXPORT
 -- --------------------------------------------------------------------------------
	 -- --------------------------------------------------------------------------------
	 -- Name: VPatientsExportData
	 -- Abstract: Shows all the patient Data
	 -- --------------------------------------------------------------------------------
	GO
	CREATE VIEW VPatientsExportData
	AS
		SELECT
			 CAST(TP.intPatientID AS VARCHAR(50)) + ';' +
			 TP.strLastName + ',' + TP.strFirstName + ' ' + TP.strMiddleName + ';' +
			 TP.strStreetAddress + ';' +
			 TP.strCity + ';' +
			 TS.strState + ';' +
			 TP.strZipcode + ';' +
			 TP.strPrimaryPhoneNumber + ';' +
			 TP.strSecondaryPhoneNumber + ';' +
			 CAST(TP.blnSmoker AS VARCHAR(50))+ ';' +
			 CAST(TP.decPackYears AS VARCHAR(50))+ ';' +
			 CAST(TP.blnHeadOfHousehold AS VARCHAR(50)) + ';' +
			 CAST(CAST(TP.dtmDateOfBirth AS DATE) AS VARCHAR(50)) + ';' + -- Leet HAXZOR5
			 TG.strSex + ';' +
			 TP.strEmailAddress + ';' +
			 TR.strFullName	+ ';' AS strTheWholeRecordDu

		FROM 
			TPatients AS TP
				INNER JOIN TStates AS TS
					ON (TP.intStateID = TS.intStateID)
				INNER JOIN TSexes AS TG
					ON (TP.intSexID = TG.intSexID)
				INNER JOIN TRecordCreators AS TR
					ON (TP.intRecordCreatorID = TR.intRecordCreatorID)
	GO


	 -- --------------------------------------------------------------------------------
	 -- Name: VPatientAllergyCount
	 -- Abstract: Gets a Count of each patients Allergies
	 -- --------------------------------------------------------------------------------
	 GO
	 CREATE VIEW VPatientAllergyCount
	 AS
	 	 SELECT
			 vPA.intPatientID,
			 COUNT(vPA.strAllergy) AS intAllergyCount
			
		 FROM	
			 VPatientAllergysData AS vPA

		 GROUP BY 
			 vPA.intPatientID
	 GO

	 SELECT * FROM VPatientAllergyMedicationCount
	 -- --------------------------------------------------------------------------------
	 -- Name: VPatientAllergyMedicationCount
	 -- Abstract: Gets a Count of each patients Allergies
	 -- --------------------------------------------------------------------------------
	 GO
	 CREATE VIEW VPatientAllergyMedicationCount
	 AS
	 	 SELECT
			 vPAM.intPatientID,
			 ISNULL(vPAM.intAllergyID, 0) AS intAllergyID, 
			 COUNT(vPAM.strMedication) AS intMedicationCountCount
			
		 FROM	
			 VPatientAllergyMedicationsData AS vPAM

		 GROUP BY 
			 vPAM.intPatientID,
			 vPAM.intAllergyID
	 GO
-- ----------------------------------------------------------------------
-- Functions
-- ----------------------------------------------------------------------

-- ----------------------------------------------------------------------
-- Stored Procedures
-- ----------------------------------------------------------------------

-- --------------------------------------------------------------------------------
-- Name: uspAddPlayer
-- Abstract: adds a Player
-- --------------------------------------------------------------------------------
GO
-- --------------------------------------------------------------------------------
-- Name: uspGetOrAddAllergy
-- Abstract: Gets and Allergy ID, if it doesnt exist it creates it then returns the ID
-- --------------------------------------------------------------------------------
GO
CREATE PROCEDURE uspGetOrAddAllergy
	  @strAllergy		VARCHAR(50)

AS
SET NOCOUNT ON		-- Report Only Errors
SET XACT_ABORT ON	-- Rollback transaction on error

BEGIN TRANSACTION

	DECLARE @intAllergyID	 INTEGER
	DECLARE @intRowCount	 INTEGER

	SET @intRowCount = (SELECT COUNT(*) FROM TAllergies WHERE strAllergy = @strAllergy)
	
	-- Does it Exist
	IF @intRowCount = 0
	BEGIN
		-- NO,

		-- Get the next highest ID and lock the table until the end of the transaction
		SELECT @intAllergyID = MAX(intAllergyID) + 1 FROM TAllergies (TABLOCKX)

		-- Default to 1 if the table is empty
		SELECT @intAllergyID = COALESCE(@intAllergyID, 1)

		-- CREATE new record
		INSERT INTO TAllergies(intAllergyID, strAllergy)
		VALUES(@intAllergyID, @strAllergy) 

		-- return ID to calling program
		SELECT @intAllergyID AS intAllergyID
	END

	-- Yes, it does exits
	IF @intRowCount > 0
	BEGIN
		SET @intAllergyID = (SELECT intAllergyID FROM TAllergies WHERE strAllergy = @strAllergy)
		
		-- return ID to calling program
		SELECT @intAllergyID AS intAllergyID
	END

COMMIT TRANSACTION
GO




CREATE PROCEDURE uspAddPatient
	 @strFirstName						VARCHAR(50)
	,@strMiddleName						VARCHAR(50)
	,@strLastName						VARCHAR(50)
	,@strStreetAddress					VARCHAR(50)
	,@strCity							VARCHAR(50)
	,@intStateID						INTEGER
	,@strZipCode						VARCHAR(50)
	,@strPrimaryPhoneNumber				VARCHAR(50)
	,@strSecondaryPhoneNumber			VARCHAR(50)
	,@blnSmoker							BIT
	,@decPackYears						DECIMAL
	,@blnHeadOfHouseHold				BIT
	,@dtmDateOfBirth					DATETIME
	,@intSexID							INTEGER
	,@intRecordCreatorID				INTEGER
	,@strEmailAddress					VARCHAR(50)
AS
SET NOCOUNT ON		-- Report Only Errors
SET XACT_ABORT ON	-- Rollback transaction on error

BEGIN TRANSACTION

	DECLARE @intPatientID	 INTEGER

	-- Get the next highest ID and lock the table until the end of the transaction
	SELECT @intPatientID = MAX(intPatientID) + 1 FROM TPatients (TABLOCKX)

	-- Is the ID null ( i.e the Table is empty )?
	IF @intPatientID IS NULL
		-- Default ID to 1
		SELECT @intPatientID = 1

	-- CREATE new record
	INSERT INTO TPatients(intPatientID, strFirstName, strMiddleName, strLastName, strStreetAddress, strCity, intStateID, strZipCode, strPrimaryPhoneNumber, strSecondaryPhoneNumber, blnSmoker, decPackYears, blnHeadOfHousehold, dtmDateOfBirth, intSexID, intRecordCreatorID, strEmailAddress)
	VALUES(@intPatientID, @strFirstName, @strMiddleName, @strLastName, @strStreetAddress, @strCity, @intStateID, @strZipCode, @strPrimaryPhoneNumber, @strSecondaryPhoneNumber, @blnSmoker, @decPackYears , @blnHeadOfHouseHold ,@dtmDateOfBirth, @intSexID, @intRecordCreatorID, @strEmailAddress)		

	-- return ID to calling program
	SELECT @intPatientID AS intPatientID

COMMIT TRANSACTION
GO


GO
-- --------------------------------------------------------------------------------
-- Name: uspGetOrAddMedication
-- Abstract: Gets and Medication ID, if it doesnt exist it creates it then returns the ID
-- --------------------------------------------------------------------------------
GO
CREATE PROCEDURE uspGetOrAddMedication
	  @strMedication		VARCHAR(50)

AS
SET NOCOUNT ON		-- Report Only Errors
SET XACT_ABORT ON	-- Rollback transaction on error

BEGIN TRANSACTION

	DECLARE @intMedicationID INTEGER
	DECLARE @intRowCount	 INTEGER

	SET @intRowCount = (SELECT COUNT(*) FROM TMedications WHERE strMedication = @strMedication)
	
	-- Does it Exist
	IF @intRowCount = 0
	BEGIN
		-- NO,

		-- Get the next highest ID and lock the table until the end of the transaction
		SELECT @intMedicationID = MAX(intMedicationID) + 1 FROM TMedications (TABLOCKX)

		-- Default to 1 if the table is empty
		SELECT @intMedicationID = COALESCE(@intMedicationID, 1)

		-- CREATE new record
		INSERT INTO TMedications(intMedicationID, strMedication)
		VALUES(@intMedicationID, @strMedication) 

		-- return ID to calling program
		SELECT @intMedicationID AS intMedicationID
	END

	-- Yes, it does exits
	IF @intRowCount > 0
	BEGIN
		SET @intMedicationID = (SELECT intMedicationID FROM TMedications WHERE strMedication = @strMedication)
		
		-- return ID to calling program
		SELECT @intMedicationID AS intMedicationID
	END

COMMIT TRANSACTION
GO


GO
-- --------------------------------------------------------------------------------
-- Name: uspGetOrAddProcedure
-- Abstract: Gets and Procedure ID, if it doesnt exist it creates it then returns the ID
-- --------------------------------------------------------------------------------
GO
CREATE PROCEDURE uspGetOrAddProcedure
	  @strProcedure		VARCHAR(50)

AS
SET NOCOUNT ON		-- Report Only Errors
SET XACT_ABORT ON	-- Rollback transaction on error

BEGIN TRANSACTION

	DECLARE @intProcedureID  INTEGER
	DECLARE @intRowCount	 INTEGER

	SET @intRowCount = (SELECT COUNT(*) FROM TProcedures WHERE strProcedure = @strProcedure)
	
	-- Does it Exist
	IF @intRowCount = 0
	BEGIN
		-- NO,

		-- Get the next highest ID and lock the table until the end of the transaction
		SELECT @intProcedureID = MAX(intProcedureID) + 1 FROM TProcedures (TABLOCKX)

		-- Default to 1 if the table is empty
		SELECT @intProcedureID = COALESCE(@intProcedureID, 1)

		-- CREATE new record
		INSERT INTO TProcedures(intProcedureID, strProcedure)
		VALUES(@intProcedureID, @strProcedure) 

		-- return ID to calling program
		SELECT @intProcedureID AS intProcedureID
	END

	-- Yes, it does exits
	IF @intRowCount > 0
	BEGIN
		SET @intProcedureID = (SELECT intProcedureID FROM TProcedures WHERE strProcedure = @strProcedure)
		
		-- return ID to calling program
		SELECT @intProcedureID AS intProcedureID
	END

COMMIT TRANSACTION
GO

-- --------------------------------------------------------------------------------
-- Name: uspGetOrAddCondition
-- Abstract: Gets and Condition ID, if it doesnt exist it creates it then returns the ID
-- --------------------------------------------------------------------------------
GO
CREATE PROCEDURE uspGetOrAddCondition
	  @strCondition		VARCHAR(50)

AS
SET NOCOUNT ON		-- Report Only Errors
SET XACT_ABORT ON	-- Rollback transaction on error

BEGIN TRANSACTION

	DECLARE @intConditionID  INTEGER
	DECLARE @intRowCount	 INTEGER

	SET @intRowCount = (SELECT COUNT(*) FROM TConditions WHERE strCondition = @strCondition)
	
	-- Does it Exist
	IF @intRowCount = 0
	BEGIN
		-- NO,

		-- Get the next highest ID and lock the table until the end of the transaction
		SELECT @intConditionID = MAX(intConditionID) + 1 FROM TConditions (TABLOCKX)

		-- Default to 1 if the table is empty
		SELECT @intConditionID = COALESCE(@intConditionID, 1)

		-- CREATE new record
		INSERT INTO TConditions(intConditionID, strCondition)
		VALUES(@intConditionID, @strCondition) 

		-- return ID to calling program
		SELECT @intConditionID AS intConditionID
	END

	-- Yes, it does exits
	IF @intRowCount > 0
	BEGIN
		SET @intConditionID = (SELECT intConditionID FROM TConditions WHERE strCondition = @strCondition)
		
		-- return ID to calling program
		SELECT @intConditionID AS intConditionID
	END

COMMIT TRANSACTION
GO


GO
-- --------------------------------------------------------------------------------
-- Name: uspGetOrAddRecordCreator
-- Abstract: Gets and RecordCreator ID, if it doesnt exist it creates it then returns the ID
-- --------------------------------------------------------------------------------
GO
CREATE PROCEDURE uspGetOrAddRecordCreator
	  @strRecordCreator		VARCHAR(50)

AS
SET NOCOUNT ON		-- Report Only Errors
SET XACT_ABORT ON	-- Rollback transaction on error

BEGIN TRANSACTION

	DECLARE @intRecordCreatorID  INTEGER
	DECLARE @intRowCount	 INTEGER

	SET @intRowCount = (SELECT COUNT(*) FROM TRecordCreators WHERE strFullName = @strRecordCreator)
	
	-- Does it Exist
	IF @intRowCount = 0
	BEGIN
		-- NO,

		-- Get the next highest ID and lock the table until the end of the transaction
		SELECT @intRecordCreatorID = MAX(intRecordCreatorID) + 1 FROM TRecordCreators (TABLOCKX)

		-- Default to 1 if the table is empty
		SELECT @intRecordCreatorID = COALESCE(@intRecordCreatorID, 1)

		-- CREATE new record
		INSERT INTO TRecordCreators(intRecordCreatorID, strFullName)
		VALUES(@intRecordCreatorID, @strRecordCreator) 

		-- return ID to calling program
		SELECT @intRecordCreatorID AS intRecordCreatorID
	END

	-- Yes, it does exits
	IF @intRowCount > 0
	BEGIN
		SET @intRecordCreatorID = (SELECT intRecordCreatorID FROM TRecordCreators WHERE strFullName = @strRecordCreator)
		
		-- return ID to calling program
		SELECT @intRecordCreatorID AS intRecordCreatorID
	END

COMMIT TRANSACTION
GO


 -- --------------------------------------------------------------------------------
 -- Name: uspAddPatientCondition
 -- Abstract: Gets Or Adds Patient Condition
 -- --------------------------------------------------------------------------------
GO
CREATE PROCEDURE uspAddPatientCondition
	 @intPatientID			AS INTEGER
	,@intConditionID		AS INTEGER

AS
SET NOCOUNT ON			-- Report only errors
SET XACT_ABORT ON		-- Terminate and rollback entire transaction on error

BEGIN TRANSACTION

	DECLARE @intRowCount	   INTEGER

	SET @intRowCount = (SELECT COUNT(*) FROM TPatientConditions WHERE intPatientID = @intPatientID AND intConditionID = @intConditionID)
	
	-- Does it Exist
	IF @intRowCount = 0
	BEGIN
		-- NO,

	-- Create New Record
	INSERT INTO TPatientConditions(intPatientID, intConditionID)
	VALUES(@intPatientID, @intConditionID) 

	END

COMMIT TRANSACTION
GO

						
 -- --------------------------------------------------------------------------------
 -- Name: uspAddPatientProcedures
 -- Abstract: Adds Patient procedure
 -- --------------------------------------------------------------------------------
GO
CREATE PROCEDURE uspAddPatientProcedure
	 @intPatientID			AS INTEGER
	,@intProcedureID		AS INTEGER

AS
SET NOCOUNT ON			-- Report only errors
SET XACT_ABORT ON		-- Terminate and rollback entire transaction on error

BEGIN TRANSACTION

	DECLARE @intRowCount	   INTEGER

	SET @intRowCount = (SELECT COUNT(*) FROM TPatientProcedures WHERE intPatientID = @intPatientID AND intProcedureID = @intProcedureID)
	
	-- Does it Exist
	IF @intRowCount = 0
	BEGIN

	-- Create New Record
	INSERT INTO TPatientProcedures(intPatientID, intProcedureID)
	VALUES(@intPatientID, @intProcedureID)
	END

COMMIT TRANSACTION
GO


 -- --------------------------------------------------------------------------------
 -- Name: uspAddPatientAllergys
 -- Abstract: Adds Patient Allergy
 -- --------------------------------------------------------------------------------
GO
CREATE PROCEDURE uspAddPatientAllergy
	 @intPatientID			AS INTEGER
	,@intAllergyID			AS INTEGER

AS
SET NOCOUNT ON			-- Report only errors
SET XACT_ABORT ON		-- Terminate and rollback entire transaction on error

BEGIN TRANSACTION

	DECLARE @intRowCount	   INTEGER

	SET @intRowCount = (SELECT COUNT(*) FROM TPatientAllergies WHERE intPatientID = @intPatientID AND intAllergyID = @intAllergyID)
	
	-- Does it Exist
	IF @intRowCount = 0
	BEGIN

	-- Create New Record
	INSERT INTO TPatientAllergies(intPatientID, intAllergyID)
	VALUES(@intPatientID, @intAllergyID)

	END

COMMIT TRANSACTION
GO


 -- --------------------------------------------------------------------------------
 -- Name: uspAddPatientConditionMedication
 -- Abstract: Adds Patient Allergy
 -- --------------------------------------------------------------------------------
GO
CREATE PROCEDURE uspAddPatientConditionMedication
	 @intPatientID			AS INTEGER
	,@intConditionID		AS INTEGER
	,@intMedicationID		AS INTEGER

AS
SET NOCOUNT ON			-- Report only errors
SET XACT_ABORT ON		-- Terminate and rollback entire transaction on error

BEGIN TRANSACTION

	DECLARE @intRowCount	   INTEGER

	SET @intRowCount = (SELECT COUNT(*) FROM TPatientConditionMedications WHERE intPatientID = @intPatientID AND intConditionID = @intConditionID AND intMedicationID = @intMedicationID)
	
	-- Does it Exist
	IF @intRowCount = 0
	BEGIN

	-- Create New Record
	INSERT INTO TPatientConditionMedications(intPatientID, intConditionID, intMedicationID)
	VALUES(@intPatientID, @intConditionID, @intMedicationID)

	END

COMMIT TRANSACTION
GO


 -- --------------------------------------------------------------------------------
 -- Name: uspAddPatientAllergyMedication
 -- Abstract: Adds Patient Allergy
 -- --------------------------------------------------------------------------------
GO
CREATE PROCEDURE uspAddPatientAllergyMedication
	 @intPatientID			AS INTEGER
	,@intAllergyID			AS INTEGER
	,@intMedicationID		AS INTEGER

AS
SET NOCOUNT ON			-- Report only errors
SET XACT_ABORT ON		-- Terminate and rollback entire transaction on error

BEGIN TRANSACTION

	DECLARE @intRowCount	   INTEGER

	SET @intRowCount = (SELECT COUNT(*) FROM TPatientAllergyMedications WHERE intPatientID = @intPatientID AND intAllergyID = @intAllergyID AND intMedicationID = @intMedicationID)
	
	-- Does it Exist
	IF @intRowCount = 0
	BEGIN

	-- Create New Record
	INSERT INTO TPatientAllergyMedications(intPatientID, intAllergyID, intMedicationID)
	VALUES(@intPatientID, @intAllergyID, @intMedicationID)

	END

COMMIT TRANSACTION
GO

-- ----------------------------------------------------------------------
-- Triggers
-- ----------------------------------------------------------------------

-- ----------------------------------------------------------------------
-- Function Permissions
-- ----------------------------------------------------------------------

-- ----------------------------------------------------------------------
-- Stored Procedure Permissions
-- ----------------------------------------------------------------------

-- ----------------------------------------------------------------------
-- Table Permissions
-- ----------------------------------------------------------------------

-- ----------------------------------------------------------------------
-- View Permissions
-- ----------------------------------------------------------------------

INSERT INTO TProcedures(intProcedureID, strProcedure)
VALUES
		 (1, 'No Procedure')


INSERT INTO TMedications(intMedicationID, strMedication)
VALUES
		 (1, 'No Medication')


INSERT INTO TConditions(intConditionID, strCondition)
VALUES
		 (1, 'No Condition')


INSERT INTO TAllergies(intAllergyID, strAllergy)
VALUES
		 (1, 'No Allergy')


INSERT INTO TSexes(intSexID, strSex)
VALUES
		 (1, 'Female')
		,(2, 'Male')

INSERT INTO TStates(intStateID, strState, strStateAbbreviation)
VALUES
		 (1,  'Alabama',		'AL')
		,(2,  'Alaska',			'AK')
		,(3,  'Arizona',		'AZ')
		,(4,  'Arkansas',		'AR')
		,(5,  'California',		'CA')
		,(6,  'Colorado',		'CO')
		,(7,  'Connecticut',	'CT')
		,(8,  'Delaware',		'DE')
		,(9,  'Florida',		'FL')
		,(10, 'Georgia',		'GA')
		,(11, 'Hawaii',			'HI')
		,(12, 'Idaho',			'ID')
		,(13, 'Illinois',		'IL')
		,(14, 'Indiana',		'IN')
		,(15, 'Iowa',			'IA')
		,(16, 'Kansas',			'KS')
		,(17, 'Kentucky',		'KY')
		,(18, 'Louisiana',		'LA')
		,(19, 'Maine',			'ME')
		,(20, 'Maryland',		'MD')
		,(21, 'Massachusetts',	'MA')
		,(22, 'Michigan',		'MI')
		,(23, 'Minnesota',		'MN')
		,(24, 'Mississippi',	'MS')
		,(25, 'Missouri',		'MO')
		,(26, 'Montana',		'MT')
		,(27, 'Nebraska',		'NE')
		,(28, 'Nevada',			'NV')
		,(29, 'New Hampshire',	'NH')
		,(30, 'New Jersey',		'NJ')
		,(31, 'New Mexico',		'NM')
		,(32, 'New York',		'NY')
		,(33, 'North Carolina',	'NC')
		,(34, 'North Dakota',	'ND')
		,(35, 'Ohio',			'OH')
		,(36, 'Oklahoma',		'OK')
		,(37, 'Oregon',			'OR')
		,(38, 'Pennsylvania',	'PA')
		,(39, 'Rhode Island',	'RI')
		,(40, 'South Carolina',	'SC')
		,(41, 'South Dakota',	'SD')
		,(42, 'Tennessee',		'TN')
		,(43, 'Texas',			'TX')
		,(44, 'Utah',			'UT')
		,(45, 'Vermont',		'VT')
		,(46, 'Virginia',		'VA')
		,(47, 'Washington',		'WA')
		,(48, 'West Virginia',	'WV')
		,(49, 'Wisconsin',		'WI')
		,(50, 'Wyoming',		'WY')
		,(51, 'American Samoa',	'AS')
		,(52, 'District of Columbia',	'DC')
		,(53, 'Federated States of Micronesia',	'FM')
		,(54, 'Guam',	'GU')
		,(55, 'Marshall Islands',	'MH')
		,(56, 'Northern Mariana Islands',	'MP')
		,(57, 'Palau',	'PW')
		,(58, 'Puerto Rico',	'PR')
		,(59, 'Virgin Islands',	'VI')
		
		SELECT * FROM VPatientsExportData
--GO
--CREATE VIEW VPatientPlusAllergyData
--AS
--			SELECT
--				 TP.strLastName + ', ' + TP.strFirstName + ' ' + TP.strMiddleName AS strPatientFullName
--				,TP.strStreetAddress
--				,TP.strCity
--				,TS.strState
--				,TP.strZipcode
--				,TP.strPrimaryPhoneNumber
--				,TP.strSecondaryPhoneNumber
--				,TP.blnSmoker
--				,TP.decPackYears
--				,TP.blnHeadOfHousehold
--				,TP.dtmDateOfBirth
--				,TG.strSex
--				,TA.strAllergy
--				,TM.strMedication		AS strAllergyMedication
--				,TP.strEmailAddress
--				,TR.strFullName			AS strRecordCreator

--			FROM 
--				TPatients AS TP
--					INNER JOIN TStates AS TS
--						ON (TP.intStateID = TS.intStateID)
--					INNER JOIN TSexes AS TG
--						ON (TP.intSexID = TG.intSexID)
--					INNER JOIN TRecordCreators AS TR
--						ON (TP.intRecordCreatorID = TR.intRecordCreatorID)
--					FULL OUTER JOIN TPatientAllergyMedications AS TPAM
--						INNER JOIN TMedications AS TM
--							ON (TPAM.intMedicationID = TM.intMedicationID)
--							INNER JOIN TPatientAllergies AS TPA
--								INNER JOIN TAllergies AS TA
--								ON (TA.intAllergyID = TPA.intAllergyID)
--							ON (TPA.intPatientID = TPAM.intPatientID
--							AND TPA.intAllergyID = TPAM.intAllergyID)
--						ON (TPAM.intPatientID = TP.intPatientID)
--GO


					
				





				--INNER JOIN TPatientConditionMedications AS TPCM
				--	INNER JOIN TPatientConditions AS TPC
				--		INNER JOIN TConditions AS TC
				--		ON TC.intConditionID = TPC.intConditionID
				--	ON  (TPC.intPatientID = TPCM.intPatientID
				--	AND  TPC.intConditionID = TPCM.intConditionID)
				--ON  TPCM.intPatientID = TP.intPatientID
				--AND TPCM.intMedicationID = TM.intMedicationID




			--TAllergies AS TA
			--	INNER JOIN TPatientAllergies AS TPA
			--		INNER JOIN TPatients AS TP
			--		ON (TPA.intPatientID = TP.intPatientID)
			--	ON(TA.intAllergyID = TPA.intAllergyID)

		 --  ,TConditions AS TC
			--	INNER JOIN TPatientConditions AS TPC
			--		INNER JOIN TPatients
			--		ON (TPC.intPatientID = TPatients.intPatientID)
			--	ON(TC.intConditionID = TPC.intConditionID)
					

		--WHERE
		--	TP.intStateID		  = TS.intStateID
		--AND TP.intSexID			  = TG.intSexID
		--AND TP.intRecordCreatorID = TR.intRecordCreatorID
		--AND TPA.intAllergyID	  = TA.intAllergyID
		--AND TPAM.intAllergyID	  = TPA.intAllergyID
		--AND TPAM.intMedicationID  = TM.intMedicationID
		--AND TPAM.intPatientID     = TPA.intPatientID
		--AND TPA.intPatientID	  = TP.intPatientID

		-- C:\Users\Admin\Desktop\ImportData - Copy.txt