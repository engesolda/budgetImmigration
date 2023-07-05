using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Globalization;
using System.Threading;

/*
 * TAGS of TextBox
 * 1- Real cash +
 * 2- Real cash -
 * 3- Dolar cash +
 * 4- Dolar cash -
 * 5- Do Nothing
 * 6- Multiply Real by 12 months +
 * 7- Multiply Real by 12 months -
 * 8- Multiply Dolar by 12 months +
 * 9- Multiply Dolar by 12 months -
 * 10- Months
 * 11- Semesters
 */



namespace BudgetImmigration
{
    public partial class NewBugdet : Form
    {
        private decimal currency = 1;
        private string lastValue = "";
        private bool locked = true;
        private bool loaded = false;
        private string loadedPath = "";

        public NewBugdet()
        {
            InitializeComponent();

            string CultureName = Thread.CurrentThread.CurrentCulture.Name;
            CultureInfo ci = new CultureInfo(CultureName);
            ci.NumberFormat.NumberDecimalSeparator = ",";
            Thread.CurrentThread.CurrentCulture = ci;

            MainTab.SelectedIndex = MainTab.TabPages.Count - 1;
            btnNewForm.BackColor = Color.LightGreen;
        }

        #region "TextBox Events"
        private void OnlyNumbers(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void SetPosition(object sender, MouseEventArgs e)
        {
            TextBox txtBox = (TextBox)sender;
            txtBox.SelectionStart = txtBox.Text.Length;
            txtBox.SelectionLength = 0;
        }

        private void Mask(object sender, KeyEventArgs e)
        {
            TextBox txtBox = (TextBox)sender;
            string newStr = "";
            string oldStr;
            int j = 0;

            oldStr = txtBox.Text;

            // Clear the string
            oldStr = oldStr.Replace(",", "");
            oldStr = oldStr.Replace(".", "");

            for (int i = oldStr.Length - 1; i >= 0; i--)
            {
                j++;

                if (j == 2)
                {
                    // In Brazil we use , for decimal
                    newStr = ',' + newStr;
                }

                if (j >= 5 && (j+1) % 3 == 0) // starting at 5, every 3 digits get a .
                {
                    newStr = '.' + newStr;
                }

                newStr = oldStr.Substring(i, 1) + newStr;
            }

            txtBox.Text = newStr;

            txtBox.SelectionStart = txtBox.Text.Length;
            txtBox.SelectionLength = 0;
        }

        private void OnEnter(object sender, EventArgs e)
        {
            lastValue = ((TextBox)sender).Text;
        }

        private void LeaveBox(object sender, EventArgs e)
        {
            TextBox txtBox = ((TextBox)sender);

            if (!String.IsNullOrEmpty(txtBox.Text))
            {
                string txt = txtBox.Text.Replace(".", "");
                txt = txt.Replace(",", "");

                // Limit sibling on the 0-12 inclusive range
                if (Convert.ToInt64(txt) > 12 && txtBox.Name.Contains("s"))
                {
                    txtBox.Text = "12";
                }

                if (Convert.ToInt64(txt) < 0 && txtBox.Name.Contains("s"))
                {
                    txtBox.Text = "0";
                }
            }

            // Only update the preview board if the value actually changed
            if (lastValue != txtBox.Text)
            {
                UpdateFinancialPreview();
            }
        }
        #endregion

        #region "Family & Pets"
        private void LabelMask(object sender, EventArgs e)
        {
            Label lblTxt = (Label)sender;
            string newStr = "";
            string oldStr;
            int j = 0;

            oldStr = lblTxt.Text;

            oldStr = oldStr.Replace("-", "");
            oldStr = oldStr.Replace(",", "");
            oldStr = oldStr.Replace(".", "");

            for (int i = oldStr.Length - 1; i >= 0; i--)
            {
                j++;

                newStr = oldStr.Substring(i, 1) + newStr;

                if (j == 2)
                {
                    newStr = ',' + newStr;
                }

                if (j == 5)
                {
                    newStr = '.' + newStr;
                }

                if (j == 8)
                {
                    newStr = '.' + newStr;
                }
            }

            if (newStr.IndexOf('.') == 0 || (newStr.IndexOf(',') == 0))
            {
                newStr = newStr.Substring(1, (newStr.Length - 1));
            }

            lblTxt.Text = newStr;

            if (lblTxt.Name == "yearPrev")
            {
                yearPrevBRL.Text = Math.Round((Convert.ToDecimal(lblTxt.Text.Replace(".","")) * currency), 2).ToString();
            }
        }
        #endregion

        #region "Fields Management"
        private void txtCurrency_Leave(object sender, EventArgs e)
        {
            currency = Convert.ToDecimal(((TextBox)sender).Text);
            currency = currency > 0 ? currency : 1;
            ((TextBox)sender).Text = currency.ToString();

            posPrev.Text = "0";
            negPrev.Text = "0";
            yearPrevCND.Text = "0";

            UpdateFinancialPreview();
        }

        /*
        * -TAGS of TextBox-
        * 1- BRL cash addition
        * 2- BRL cash removal
        * 3- CND cash addition
        * 4- CND cash removal
        * 5- Do Nothing
        
        * -TextBox naming system-
        * Text boxes are named using "tab" + tab# + f + "f#" where f means field.
        * So the first textBox of the first tab would be named tab1f1.
        * If the field requires a month field to count how many month that field has to be multiplied then the sibling field will be named as following:
        * "tab" + tab# + "f" + f# + "s" where s means sibling.
        * So the sibiling field that holds the # of months for field 9 of tab 9 would be tab9f9s.
        */
        private void UpdateFinancialPreview ()
        {
            posPrev.Text = "0";
            negPrev.Text = "0";
            yearPrevCND.Text = "0";
            decimal yearPositive = 0;
            decimal yearNegative = 0;
            decimal yearTotalCND = 0;
            decimal tempValue = 0;

            foreach (TabPage tab in MainTab.TabPages)
            {
                foreach (Control control in tab.Controls)
                {
                    tempValue = 0;

                    if (control.GetType().ToString() == "System.Windows.Forms.TextBox")
                    {
                        TextBox txtBox = ((TextBox)control);

                        // We only want fields following tabXfY naming
                        if (!String.IsNullOrEmpty(txtBox.Text) && txtBox.Name != "txtCurrency" && txtBox.Name != "txtMoneySaved" && !txtBox.Name.Contains("s"))
                        {
                            tempValue = Convert.ToDecimal(txtBox.Text.Replace(".", "")); // Revome de dots but leave the , so the decimal cast still works with decimal

                            // First I just get the value of the field
                            if (txtBox.Tag != null && (txtBox.Tag.ToString() == "1" || txtBox.Tag.ToString() == "2" ||
                                                            txtBox.Tag.ToString() == "3" || txtBox.Tag.ToString() == "4"))
                            {
                                foreach (Control sibling in tab.Controls)
                                {
                                    // If its a monthly occurrence   
                                    if (sibling.Name == control.Name + "s")
                                    {
                                        tempValue *= Convert.ToDecimal(sibling.Text);
                                    }
                                }
                            }

                            // If its BRL I need to change the currency
                            if (txtBox.Tag != null && (txtBox.Tag.ToString() == "1" || txtBox.Tag.ToString() == "2"))
                            {
                                tempValue /= currency;
                            }

                            // If its an expense this value must be negative
                            if (txtBox.Tag != null && (txtBox.Tag.ToString() == "2" || txtBox.Tag.ToString() == "4"))
                            {
                                yearNegative += tempValue;
                                tempValue *= -1;
                            } else
                            {
                                yearPositive += tempValue;
                            }
                        }
                    }

                    yearTotalCND += tempValue;
                }
            }

            setYearPrevText(yearPositive, yearNegative, yearTotalCND);
        }
        public void setYearPrevText(decimal _yearPositive, decimal _yearNegative, decimal _yearTotalCND)
        {
            decimal yearProfit = _yearPositive - _yearNegative;
            posPrev.Text = _yearPositive.ToString("F");
            negPrev.Text = _yearNegative.ToString("F");
            yearPrevCND.Text = _yearTotalCND.ToString("F");
            yearPrevBRL.Text = Math.Round(yearProfit * currency, 2).ToString("F");

            if (_yearPositive >= _yearNegative)
            {
                yearPrevCND.BackColor = Color.Lime;
                lblDolar.BackColor = Color.Lime;

                yearPrevBRL.BackColor = yearPrevCND.BackColor;
                lblReal.BackColor = lblDolar.BackColor;
            }
            else
            {
                yearPrevCND.BackColor = Color.FromArgb(192, 0, 0);
                lblDolar.BackColor = Color.FromArgb(192, 0, 0);

                yearPrevBRL.BackColor = yearPrevCND.BackColor;
                lblReal.BackColor = lblDolar.BackColor;
            }
        }
        #endregion

        #region "Links"
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("http://vivendonocanada.com.br");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to open link that was clicked.");
            }
        }

        private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("https://www.youtube.com/c/VivendoNoCanadáBR");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to open link that was clicked.");
            }
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("https://www.instagram.com/vivendonocanada_/");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to open link that was clicked.");
            }
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("https://www.facebook.com/blogvivendonocanada/");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to open link that was clicked.");
            }
        }
        #endregion

        #region "Filing System"
        private void NewBugdet_Load(object sender, EventArgs e)
        {

        }

        private void MainTab_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (locked)
            {   
                MainTab.TabPages["Menu"].Focus();
                MainTab.SelectedIndex = MainTab.TabPages.Count - 1;
            }
        }

        private void btnNewForm_Click(object sender, EventArgs e)
        {
            if (locked == false)
            {
                DialogResult answer = MessageBox.Show("Iniciar um novo planejamento vai apagar todo conteúdo não salvo até o momento. Você tem certeza disso ?", "Aviso", MessageBoxButtons.YesNo);
                if (answer == DialogResult.Yes)
                {
                    // Clean All
                    foreach (TabPage tab in MainTab.TabPages)
                    {
                        foreach (Control control in tab.Controls)
                        {
                            if (control.GetType().ToString() == "System.Windows.Forms.TextBox")
                            {
                                TextBox txtbox = ((TextBox)control);

                                txtbox.Text = "";

                                if (txtbox.Tag != null && txtbox.Name.Contains("s"))
                                {
                                    txtbox.Text = "12";
                                }

                                if (txtbox.Name == "txtCurrency")
                                {
                                    txtbox.Text = "1";
                                }
                            }
                        }
                    }

                    yearPrevCND.Text = "000.000,00";
                    negPrev.Text = "000.000,00";
                    posPrev.Text = "000.000,00";
                    yearPrevBRL.Text = "000.000,00";

                    lblDolar.BackColor = lblReal.BackColor = yearPrevCND.BackColor = yearPrevBRL.BackColor = Color.Transparent;

                    SetFocusFirstTab();
                }
            }
            else
            {
                locked = false;
                Tutorial tutorial = new Tutorial();
                tutorial.ShowDialog();
                btnNewForm.BackColor = btnSave.BackColor;
                SetFocusFirstTab();
            }
        }

        private void SetFocusFirstTab ()
        {
            MainTab.TabPages[0].Focus();
            MainTab.SelectedIndex = 0;
        }

        private void btnTutorial_Click(object sender, EventArgs e)
        {
            Tutorial tutorial = new Tutorial();
            tutorial.ShowDialog();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            DialogResult answer = MessageBox.Show("Você já salvou tudo que precisava?", "Aviso", MessageBoxButtons.YesNo);
            if (answer == DialogResult.Yes)
            {
                this.Close();
            }
        }

        /*
         * Simple save using text file, this does not contain sensitive information so nothing complex is necessary here
         */
        private void btnSave_Click(object sender, EventArgs e)
        {
            //Override the loaded file
            if (loaded)
            {
                saveFile(loadedPath);
            }
            else // New save
            {
                try
                {
                    // You cannot save until you started a new form
                    if (locked == false)
                    {
                        DialogResult result;
                        string pathChoose = "";

                        result = savePath.ShowDialog();
                        pathChoose = savePath.FileName;
                        pathChoose += ".txt";

                        if (result == DialogResult.OK)
                        {
                            saveFile(pathChoose);
                        } else
                        {
                            // TODO
                        }
                    }
                    else
                    {
                        MessageBox.Show("Você ainda não carregou nenhum arquivo nem iniciou um novo planejamento.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao tentar salvar o arquivo: " + ex.ToString());
                }
            }
        }

        private void btnSaveAs_Click(object sender, EventArgs e)
        {
            if (locked == false)
            {
                DialogResult result;
                string pathChoose = "";

                result = savePath.ShowDialog();
                pathChoose = savePath.FileName;

                // Just double checking
                if (!pathChoose.Contains(".txt"))
                {
                    pathChoose += ".txt";
                }

                if (result == DialogResult.OK)
                {
                    saveFile(pathChoose);
                }
            }
            else
            {
                MessageBox.Show("Você ainda não carregou nenhum arquivo nem iniciou um novo planejamento.");
            }
        }

        private void saveFile(string _path)
        {
            try
            {
                string save = "";

                foreach (TabPage tab in MainTab.TabPages)
                {
                    foreach (Control control in tab.Controls)
                    {
                        if (control.GetType().ToString() == "System.Windows.Forms.TextBox")
                        {
                            save += ((TextBox)control).Name + "-" + ((TextBox)control).Text + ";";
                        }
                    }
                }

                File.WriteAllText(@_path, save);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao tentar salvar o arquivo: " + ex.ToString());
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            DialogResult question = DialogResult.Yes;

            if (locked == false)
            {
                question = MessageBox.Show("Isso vai apagar os dados atuais, deseja prosseguir?", "Atenção", MessageBoxButtons.YesNo);
            }

            if (question == DialogResult.Yes || locked)
            {
                DialogResult result = fileLoad.ShowDialog();

                if (result == DialogResult.OK)
                {
                    string loadedFile = File.ReadAllText(fileLoad.FileName);

                    foreach (string str in loadedFile.Split(';'))
                    {
                        string[] boxInfo = str.Split('-');

                        foreach (TabPage tab in MainTab.TabPages)
                        {
                            foreach (Control control in tab.Controls)
                            {
                                if (control.GetType().ToString() == "System.Windows.Forms.TextBox")
                                {
                                    if (boxInfo[0] == ((TextBox)control).Name)
                                    {
                                        ((TextBox)control).Text = boxInfo[1];
                                    }
                                }
                            }
                        }
                    }

                    loaded = true;
                    locked = false;
                    SetFocusFirstTab();
                    loadedPath = fileLoad.FileName;
                    txtMoneySaved.Focus();
                } else
                {
                    // TODO
                }
            } 
        }
    }
    #endregion
}

