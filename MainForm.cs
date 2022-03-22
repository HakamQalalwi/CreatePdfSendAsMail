using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace CreatePdfSendAsMail
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("The text you wrote will be sent as PDF. Do you confirm?","Confirm",MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string pdfFilePath = @"C:\Users\" + SystemInformation.UserName + @"\Desktop\" + tbxPdfFileName.Text + ".pdf";

                    SmtpClient client = new SmtpClient()
                    {
                        Host = "smtp.gmail.com",
                        Port = 587,
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential()
                        {
                            UserName = "arsene.lupin9901@gmail.com",
                            Password = "1999202179"
                        }
                    };

                    MailAddress fromEmail = new MailAddress("arsene.lupin9901@gmail.com", "Hakam Qalalwi");
                    MailAddress toEmail = new MailAddress(tbxEmailAddress.Text);

                    MailMessage mailMessage = new MailMessage()
                    {
                        From = fromEmail,
                        Subject = tbxSubject.Text,
                        Body = rtbMailMessage.Text
                    };

                    CreatePdf(pdfFilePath);

                    mailMessage.Attachments.Add(new Attachment(pdfFilePath));
                    mailMessage.To.Add(toEmail);


                    //// syncronic /////////////////////////////////////////////////////////////
                    //try
                    //{
                    //    client.Send(mailMessage);
                    //    MessageBox.Show("Mail sent successfully.","Information",MessageBoxButtons.OK,MessageBoxIcon.Information);
                    //}
                    //catch (Exception exception)
                    //{
                    //    MessageBox.Show("An error occurred!\n" + "Error message: " + exception.Message, "Error",MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //}
                    ////////////////////////////////////////////////////////////////////////////


                    //// asyncronic ///////
                    client.SendMailAsync(mailMessage);
                    client.SendCompleted += Client_SendCompleted;

                    System.Threading.Thread.Sleep(2000);

                    client.Dispose();
                    mailMessage.Dispose();

                    if (MessageBox.Show("Delete PDF file created on desktop?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        File.Delete(pdfFilePath);
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show("An exception occurred while the application was running!\n" + "Exception message: " + exception.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void ClearAllControls()
        {
            tbxEmailAddress.Text = "";
            tbxPdfFileName.Text = "";
            tbxSubject.Text = "";
            rtbMailMessage.Text = "";
            rtbPdfText.Text = "";
        }

        private void Client_SendCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show("An error occurred!\n" + "Error message: " + e.Error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
                MessageBox.Show("Mail sent successfully.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void CreatePdf(string filePath)
        {
            iTextSharp.text.Document pdfDoc = new iTextSharp.text.Document();
            PdfWriter.GetInstance(pdfDoc, new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite));

            // For Turkish Characters
            iTextSharp.text.pdf.BaseFont STF_Helvetica_Turkish = BaseFont.CreateFont("Helvetica", "CP1254", BaseFont.NOT_EMBEDDED);
            iTextSharp.text.Font fontTitle = new iTextSharp.text.Font(STF_Helvetica_Turkish, 12, iTextSharp.text.Font.NORMAL);

            pdfDoc.AddAuthor("FirstName Lastname");
            pdfDoc.AddCreationDate();
            pdfDoc.AddHeader(tbxPdfFileName.Text, tbxSubject.Text);
            if (pdfDoc.IsOpen() == false)
            {
                pdfDoc.Open();
            }
            pdfDoc.Add(new Paragraph(rtbPdfText.Text, fontTitle));
            pdfDoc.Close();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearAllControls();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnMaximized_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized)
            {
                this.WindowState = FormWindowState.Normal;
                return;
            }
            this.WindowState = FormWindowState.Maximized;
        }

        private void btnMinimized_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void tbxEmailAddress_Leave(object sender, EventArgs e)
        {
            string pattern = "^([0-9a-zA-Z]([-\\.\\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\\w]*[0-9a-zA-Z]\\.)+[0-9a-zA-Z]{2,9})$";
            if (Regex.IsMatch(tbxEmailAddress.Text,pattern))
            {
                errorProvider1.Clear();
            }
            else
            {
                errorProvider1.SetError(this.tbxEmailAddress, "Incorrect email address entry");
                tbxEmailAddress.ForeColor = Color.Red;
                tbxEmailAddress.Text = "Please enter the e-mail address in the correct format.";
            }
        }

        private void tbxEmailAddress_Click(object sender, EventArgs e)
        {
            tbxEmailAddress.Text = "";
            tbxEmailAddress.ForeColor = Color.Black;
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            tbxEmailAddress.Focus();
        }

        private void tbxEmailAddress_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                tbxSubject.Focus();
            }
        }

        private void tbxSubject_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                rtbMailMessage.Focus();
            }
        }

        private void tbxPdfFileName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                rtbPdfText.Focus();
            }
        }
    }
}
