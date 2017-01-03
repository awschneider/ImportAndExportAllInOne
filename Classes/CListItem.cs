using System;

namespace nsImportAndExportAllInOne
{

    // --------------------------------------------------------------------------------
    // Class: CListItem
    // Author: Patrick Callahan
    // Abstract: When adding an item to a list we need to store 2 pieces of information:
    // -name: text part of item for user
    // -ID: unique identifier for item for programmer to limit the
    // effects of DML commands to a single row/record
    //
    // The name is useless without the ID and the ID is useless without the name.
    // So we package them up in either a structure or a class.
    // Since we want to display these together in a listbox or combobox
    // we need to override the ToString method to display the name.
    // Since we have a procedure and variables we should use a class
    // instead of a structure.
    //
    // Revision Owner Changes:
    //1 2003/04/17 P.C. Created
    //2 2007/08/13 P.C. Updated to .Net 2.0
    //3 2012/06/06 P.C. Updated to .Net 4.0 and Windows 7
    // --------------------------------------------------------------------------------
    public class CListItem
    {
        // --------------------------------------------------------------------------------
        // Properties
        // --------------------------------------------------------------------------------
        private int m_intID = 0;
        private string m_strName = "";


        // --------------------------------------------------------------------------------
        // --------------------------------------------------------------------------------
        // Methods
        // --------------------------------------------------------------------------------
        // --------------------------------------------------------------------------------



        // --------------------------------------------------------------------------------
        // Name: CListItem
        // Abstract: Default constructor
        // --------------------------------------------------------------------------------
        public CListItem()
        {
            try
            {
                m_intID = 0;
                m_strName = "";
            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }
        }



        // --------------------------------------------------------------------------------
        // Name: New
        // Abstract: Parameterized constructor
        // --------------------------------------------------------------------------------
        public CListItem(int intID, string strName)
        {
            try
            {
                Initialize(intID, strName);
            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }
        }



        // --------------------------------------------------------------------------------
        // Name: GetID
        // Abstract: Get the name property
        // --------------------------------------------------------------------------------
        public int GetID()
        {
            try
            {

            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }

            return m_intID;
        }




        // --------------------------------------------------------------------------------
        // Name: SetID
        // Abstract: Set the name property
        // --------------------------------------------------------------------------------
        public void SetID(int intID)
        {

            try
            {
                m_intID = intID;
            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }
        }



        // --------------------------------------------------------------------------------
        // Name: Initialize
        // Abstract: Set ID and name
        // --------------------------------------------------------------------------------
        public void Initialize(int intID, string strName)
        {
            try
            {
                SetID(intID);

                SetName(strName);
            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }
        }



        // --------------------------------------------------------------------------------
        // Name: GetName
        // Abstract: Get the name property
        // --------------------------------------------------------------------------------
        public string GetName()
        {
            try
            {

            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }

            return m_strName;
        }



        // --------------------------------------------------------------------------------
        // Name: SetName
        // Abstract: Set the name property
        // --------------------------------------------------------------------------------
        public void SetName(string strName)
        {
            try
            {
                m_strName = strName;
            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }
        }



        // --------------------------------------------------------------------------------
        // Name: ToString
        // Abstract: Return the name property. This is what gets displayed in
        // a listbox or combobox. Each instance will have its own
        // string.
        // --------------------------------------------------------------------------------
        public override string ToString()
        {
            string strStringToDisplayInListBoxOrComboBox = "";

            try
            {
                strStringToDisplayInListBoxOrComboBox = m_strName;
            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }

            return strStringToDisplayInListBoxOrComboBox;
        }
    }
}

