using System;
using System.Windows.Forms;

namespace nsImportAndExportAllInOne
{
    // --------------------------------------------------------------------------------
    // Name: Form1()
    // Abstract: Component Initalization
    // --------------------------------------------------------------------------------
    public partial class Form1 : Form
    {
        public Form1()
        {
            try
            {
                InitializeComponent();
            }
            catch(Exception excError)
            {
                CUtilities.WriteLog(excError);
            }
        }



        // --------------------------------------------------------------------------------
        // Name: FMain_Load
        // Abstract: Called when the form is displayed.  Perform some initialization.
        // --------------------------------------------------------------------------------
        private void FMain_Load(object sender, EventArgs e)
        {
            try
            {
                // Allow only one instance of the application to run
                if (CUtilities.CheckForPreviousApplicationInstance() == true) this.Close();

                // We are busy
                CUtilities.SetBusyCursor(this, true);

                // Can we connect to the database?
                if (CDatabaseUtilities.OpenDatabaseConnection() == false)
                {
                    // No, Warn the user
                    MessageBox.Show(this, "Database connection error." + "\n" +
                                    "The application will now close.",
                                    this.Text + " Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch (Exception excError)
            {
                // Log and display the error
                CUtilities.WriteLog(excError);
            }
            finally
            {
                // We are NOT busy
                CUtilities.SetBusyCursor(this, false);
            }

        }


        // -------------------------------------------------------------------------
        // Name: btnImport_Click
        // Abstract: Bring up the Import form
        // -------------------------------------------------------------------------
        private void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                // Create an instnace
                Form frmImport = null;

                // assign the instance
                frmImport = new FImport();

                // Show modally
                frmImport.ShowDialog();
            }
            catch(Exception excError)
            {
                CUtilities.WriteLog(excError);
            }
        }



        // -------------------------------------------------------------------------
        // Name: btnExport_Click
        // Abstract: Bring up the Export form
        // -------------------------------------------------------------------------
        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                // Create an instnace
                Form frmExport = null;

                // assign the instance
                frmExport = new FExport();

                // Show modally
                frmExport.ShowDialog();
            }
            catch (Exception excError)
            {
                CUtilities.WriteLog(excError);
            }
        }
    }
}
