using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using HPS.SAML.Library;
using System.Security.Cryptography.X509Certificates;

namespace saml
{
    public partial class IdPLauncher : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }


        protected void btnLaunchSSO_Click(object sender, EventArgs e)
        {
            // Set RelayState - Target Resource
            RelayState.Value = txtTarget.Text;

            // Create SAML Response and set Form Value
            // Collect SAML Attributes for packing into assertion
            Dictionary<string, string> SAMLAttributes = new Dictionary<string, string>();
            foreach (System.Web.UI.HtmlControls.HtmlTableRow tr in tblAttrs.Rows)
            {
                if (tr.Cells[1].Controls.Count > 1)
                {
                    TextBox AttrKey = (TextBox)tr.Cells[0].Controls[1];
                    if (!string.IsNullOrEmpty(AttrKey.Text))
                    {
                        TextBox AttrValue = (TextBox)tr.Cells[1].Controls[1];
                        CheckBox SendNull = (CheckBox)tr.Cells[2].Controls[1];
                        if (SendNull.Checked)
                        {
                            if (string.IsNullOrEmpty(AttrValue.Text))
                            {
                                SAMLAttributes.Add(((TextBox)tr.Cells[0].Controls[1]).Text, null);
                            }
                            else
                            {
                                SAMLAttributes.Add(((TextBox)tr.Cells[0].Controls[1]).Text, AttrValue.Text);
                            }
                        }
                        else
                        {
                            SAMLAttributes.Add(((TextBox)tr.Cells[0].Controls[1]).Text, AttrValue.Text);
                        }
                    }
                }
            }

            // get the certificate
            String CertPath = System.Web.Hosting.HostingEnvironment.MapPath(@"~/App_Data/HPSmylearningplan.pfx");
            X509Certificate2 SigningCert = new X509Certificate2(CertPath, "CERT PASS GOES HERE");

            // Add base 64 encoded SAML Response to form for POST
            String NameID = String.Empty;
            if (!string.IsNullOrEmpty(txtNameID.Text))
            {
                NameID = txtNameID.Text;
            }

            SAMLResponse.Value = SAML20Assertion.CreateSAML20Response(
               txtIssuer.Text, 5, "Audience", NameID, "Recipient", SAMLAttributes, SigningCert);

            // Set Body page load action
            frmIdPLauncher.Action = txtSPURL.Text;

            // add javascript to HTTP POST to the SSO configured
            // This implements the IdP-initiated HTTP POST use case
            HtmlGenericControl body = (HtmlGenericControl)this.Page.FindControl("bodySSO");
            if (body != null)
            {
                body.Attributes.Add("onload", "document.forms.frmIdPLauncher.submit();");
            }

        }

        #region Page Admin and Maintenance

        protected void btnClearAttr_Click(object sender, EventArgs e)
        {
            ClearAttributes();
        }

        private void ClearAttributes()
        {
            foreach (System.Web.UI.HtmlControls.HtmlTableRow tr in tblAttrs.Rows)
            {
                if (tr.Cells[1].Controls.Count > 1)
                {
                    TextBox AttrValue = (TextBox)tr.Cells[1].Controls[1];
                    AttrValue.Text = string.Empty;
                }
            }

        }

        private void LoadAttributes(List<KeyValuePair<string, string>> AttributeValues)
        {
            foreach (KeyValuePair<string, string> AttributeValue in AttributeValues)
            {
                foreach (System.Web.UI.HtmlControls.HtmlTableRow tr in tblAttrs.Rows)
                {
                    if (tr.Cells[1].Controls.Count > 1)
                    {
                        if (((TextBox)tr.Cells[0].Controls[1]).Text.Equals(AttributeValue.Key))
                        {
                            TextBox AttrValue = (TextBox)tr.Cells[1].Controls[1];
                            AttrValue.Text = AttributeValue.Value;
                        }
                    }
                }
            }
        }

        protected void btnLoadSamp1_Click(object sender, EventArgs e)
        {
            List<KeyValuePair<string, string>> lsUser = new List<KeyValuePair<string, string>>();
            lsUser.Add(new KeyValuePair<string, string>("UserID", "10"));
            lsUser.Add(new KeyValuePair<string, string>("Username", "akaba@harmonytx.org"));
            lsUser.Add(new KeyValuePair<string, string>("UserFirstName", "Ali"));
            lsUser.Add(new KeyValuePair<string, string>("UserLastName", "Kaba"));
            lsUser.Add(new KeyValuePair<string, string>("UserEmail", "akaba@harmonytx.org"));
            LoadAttributes(lsUser);
        }

        protected void btnLoadSamp2_Click(object sender, EventArgs e)
        {
            List<KeyValuePair<string, string>> lsUser = new List<KeyValuePair<string, string>>();
            lsUser.Add(new KeyValuePair<string, string>("UserID", "66"));
            lsUser.Add(new KeyValuePair<string, string>("Username", "ikara@harmonytx.org"));
            lsUser.Add(new KeyValuePair<string, string>("UserFirstName", "Ihsan"));
            lsUser.Add(new KeyValuePair<string, string>("UserLastName", "Kara"));
            lsUser.Add(new KeyValuePair<string, string>("UserEmail", "ikara@harmonytx.org"));
            LoadAttributes(lsUser);
        }





        protected void btnClearChecks_Click(object sender, EventArgs e)
        {
            foreach (System.Web.UI.HtmlControls.HtmlTableRow tr in tblAttrs.Rows)
            {
                if (tr.Cells[1].Controls.Count > 1)
                {
                    CheckBox SubmitNull = (CheckBox)tr.Cells[2].Controls[1];
                    SubmitNull.Checked = false;
                }
            }
        }

        #endregion

    }
}