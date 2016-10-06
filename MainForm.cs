using System;
using System.Windows.Forms;

namespace MANGO_WS_TEST
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.ActiveControl = textRequest;
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            // Check textRequest is empty
            if(string.IsNullOrEmpty(textRequest.Text))
            {
                MessageBox.Show("Please copy and paster xml request!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Read XML
            if(!Helper.IsXMLValid(textRequest.Text))
            {
                MessageBox.Show("Invalid XML!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Call HTTP Post request
            string response = string.Empty;
            try
            {
                response = Helper.CallWMSOrderNotifyWebService(textRequest.Text);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Update Reponse Text box
            textResponse.Text = response + "\n\n\n" + Helper.GetBase64Decoding(response);

        }
    }
}
