// -------------------------------------------------------------------------
// Name: FImportRecords
// Abstract: A Form for importing patient records
// -------------------------------------------------------------------------

// -------------------------------------------------------------------------
// Imports
// -------------------------------------------------------------------------
using System;
using System.Windows.Forms;


namespace nsImportAndExportAllInOne
{
    public partial class FImport : Form
    {
        // -------------------------------------------------------------------------
        // Name: FImportRecords
        // Abstract: Form Constructor
        // -------------------------------------------------------------------------
        public FImport()
        {
            InitializeComponent();
        }



        // --------------------------------------------------------------------------------
        // Name: FImport_Load
        // Abstract: Called when the form is displayed.  Perform some initialization.
        // --------------------------------------------------------------------------------
        private void FImport_Load(object sender, EventArgs e)
        {
            try
            {
                // Allow only one instance of the application to run
                if (CUtilities.CheckForPreviousApplicationInstance() == true) this.Close();

            }
            catch (Exception excError)
            {
                // Log and display the error
                CUtilities.WriteLog(excError);
            }

        }




        // --------------------------------------------------------------------------------
        // Name: btnBrowse_Click
        // Abstract: Browse to a file
        // --------------------------------------------------------------------------------
        private void btnBrowse_Click(object sender, EventArgs e)
        {
            try
            {
                string strFilePath = "";

                OpenFileDialog filOpenFileDialog = null;

                filOpenFileDialog = new OpenFileDialog();

                // Open Dialog result
                // Was a file selected?
                if (filOpenFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Yes, Then save the path
                    strFilePath = filOpenFileDialog.FileName;

                    // Assign the File path to the label
                    txtFilePath.Text = strFilePath;
                }
            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }
        }



        // --------------------------------------------------------------------------------
        // Name: btnImport_Click
        // Abstract: Import record from a file to a database
        // --------------------------------------------------------------------------------
        private void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                // Was the Import Successful?
                if (CImportUtilities.ImportPatientRecords(txtFilePath.Text) == true)
                {
                    // Yes
                    MessageBox.Show("Import was Successful!");
                }
            }
            catch(Exception excError)
            {
                CUtilities.WriteLog(excError);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {

        }

    }
}
