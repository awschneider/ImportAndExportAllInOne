using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace nsImportAndExportAllInOne
{
    public partial class FExport : Form
    {
        public FExport()
        {
            InitializeComponent();
        }

        // --------------------------------------------------------------------------------
        // Name: FExport_Load
        // Abstract: Called when the form is displayed.  Perform some initialization.
        // --------------------------------------------------------------------------------
        private void FExport_Load(object sender, EventArgs e)
        {
            try
            {
                // Allow only one instance of the application to run
                if (CUtilities.CheckForPreviousApplicationInstance() == true) this.Close();
                else
                {
                    CExportUtilities.ExportPatientRecords("Not a real file path.c");                    // *******  DEBUG  *******
                }

            }
            catch (Exception excError)
            {
                // Log and display the error
                CUtilities.WriteLog(excError);
            }

        }
    }
}
