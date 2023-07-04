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

        #region "TextBox General Events"
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

                if (Convert.ToInt64(txt) > 12 && txtBox.Tag.ToString() == "10")
                {
                    txtBox.Text = "12";
                }

                if (Convert.ToInt64(txt) < 0 && txtBox.Tag.ToString() == "10")
                {
                    txtBox.Text = "0";
                }

                if (Convert.ToInt64(txt) > 2 && txtBox.Tag.ToString() == "11")
                {
                    txtBox.Text = "2";
                }

                if (Convert.ToInt64(txt) < 0 && txtBox.Tag.ToString() == "11")
                {
                    txtBox.Text = "0";
                }
            }

            // Only update if the value actually changed
            if (lastValue != txtBox.Text)
            {
                UpdateFinancialPreview();
            }
        }
        #endregion

        #region "Family & Pets"
        /*private void txtAdulto_TextChanged(object sender, EventArgs e)
        {
            adults = Convert.ToInt64(((TextBox)sender).Text);
        }

        private void txtKid_TextChanged(object sender, EventArgs e)
        {
            kids = Convert.ToInt64(((TextBox)sender).Text);
        }

        private void txtBigDog_TextChanged(object sender, EventArgs e)
        {
            bDogs = Convert.ToInt64(((TextBox)sender).Text);
        }

        private void txtSmallDog_TextChanged(object sender, EventArgs e)
        {
            sDogs = Convert.ToInt64(((TextBox)sender).Text);
        }
        
        private void txtCat_TextChanged(object sender, EventArgs e)
        {
            cats = Convert.ToInt64(((TextBox)sender).Text);
        }*/

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
                lblAnoReal.Text = Math.Round((Convert.ToDecimal(lblTxt.Text.Replace(".","")) * currency), 2).ToString();
            }
        }
        #endregion

        private void txtCurrency_Leave(object sender, EventArgs e)
        {
            currency = Convert.ToDecimal(((TextBox)sender).Text);

            posPrev.Text = "0";
            negPrev.Text = "0";
            yearPrev.Text = "0";

            UpdateFinancialPreview();
        }

        public void ChangeFinancialPreview(decimal _amount, string _operation, long _months)
        {
            decimal yearValue;
            decimal positive = 0;
            decimal negative = 0;

            if (_operation == "add")
            {
                positive = String.IsNullOrEmpty(posPrev.Text) ? 0 : Convert.ToDecimal(posPrev.Text.Replace(".", ""));

                positive += (_amount * _months);
                positive = Math.Round(positive, 2);
                posPrev.Text = positive.ToString();
                negative = Convert.ToDecimal(negPrev.Text.Replace(".", ""));
            }
            if (_operation == "sub")
            {
                negative = String.IsNullOrEmpty(negPrev.Text) ? 0 : Convert.ToDecimal(negPrev.Text.Replace(".", ""));

                negative += (_amount * _months);
                negative = Math.Round(negative, 2);
                negPrev.Text = negative.ToString();
                positive = Convert.ToDecimal(posPrev.Text.Replace(".", ""));
            }

            yearValue = Math.Round(_amount * _months, 2);
            yearPrev.Text = (positive - negative).ToString();

            if (Convert.ToDecimal(posPrev.Text.Replace(".", "")) > Convert.ToDecimal(negPrev.Text.Replace(".","")))
            {
                yearPrev.BackColor = Color.Lime;
                lblDolar.BackColor = Color.Lime;

                lblAnoReal.BackColor = yearPrev.BackColor;
                lblReal.BackColor = lblDolar.BackColor;
            }
            if ((Convert.ToDecimal(posPrev.Text.Replace(".", "")) < Convert.ToDecimal(negPrev.Text.Replace(".", ""))))
            {
                yearPrev.BackColor = Color.FromArgb(192, 0, 0);
                lblDolar.BackColor = Color.FromArgb(192, 0, 0);

                lblAnoReal.BackColor = yearPrev.BackColor;
                lblReal.BackColor = lblDolar.BackColor;
            }
        }
        
        private void UpdateFinancialPreview ()
        {
            posPrev.Text = "0";
            negPrev.Text = "0";
            yearPrev.Text = "0";
            long months = 12;

            foreach (TabPage tab in MainTab.TabPages)
            {
                foreach (Control control in tab.Controls)
                {
                    if (control.GetType().ToString() == "System.Windows.Forms.TextBox")
                    {
                        TextBox txtBox = ((TextBox)control);
                        string value = txtBox.Text.Replace(".","");
                        //value = value.Replace(",", ".");

                        if (!String.IsNullOrEmpty(txtBox.Text))
                        {
                            // 1- BRL cash +
                            if (txtBox.Tag != null && txtBox.Tag.ToString() == "1")
                            {
                                ChangeFinancialPreview(Convert.ToDecimal(value) / currency, "add", 1);
                            }

                            // 2- BRL cash -
                            if (txtBox.Tag != null && txtBox.Tag.ToString() == "2")
                            {
                                ChangeFinancialPreview(Convert.ToDecimal(value) / currency, "sub", 1);
                            }

                            // 3- Dolar cash +
                            if (txtBox.Tag != null && txtBox.Tag.ToString() == "3")
                            {
                                ChangeFinancialPreview(Convert.ToDecimal(value), "add", 1);
                            }

                            // 4- Dolar cash -
                            if (txtBox.Tag != null && txtBox.Tag.ToString() == "4")
                            {
                                ChangeFinancialPreview(Convert.ToDecimal(value), "sub", 1);
                            }

                            // 6- Multiply BRL cash by 12 months +
                            if (txtBox.Tag != null && txtBox.Tag.ToString() == "6")
                            {
                                foreach (Control box in tab.Controls)
                                {
                                    if (box.GetType().ToString() == "System.Windows.Forms.TextBox")
                                    {
                                        // First box contains "txtBox1" the second must have the name "txtBox11"
                                        string name = txtBox.Name + txtBox.Name.Substring(txtBox.Name.Length-1,1);
                                        if (((TextBox)box).Name.Contains(name))
                                        {
                                            months = Convert.ToInt64(((TextBox)box).Text);
                                        }
                                    }
                                }

                                ChangeFinancialPreview(Convert.ToDecimal(value) / currency, "add", months);
                            }

                            // 7 - Multiply Real cash by 12 months -
                            if (txtBox.Tag != null && txtBox.Tag.ToString() == "7")
                            {
                                foreach (Control box in tab.Controls)
                                {
                                    if (box.GetType().ToString() == "System.Windows.Forms.TextBox")
                                    {
                                        // First box contains "txtBox1" the second must have the name "txtBox11"
                                        string name = txtBox.Name + txtBox.Name.Substring(txtBox.Name.Length - 1, 1);
                                        if (((TextBox)box).Name.Contains(name))
                                        {
                                            months = Convert.ToInt64(((TextBox)box).Text);
                                        }
                                    }
                                }

                                ChangeFinancialPreview(Convert.ToDecimal(value) / currency, "sub", months);
                            }

                            // 8- Multiply Dolar cash by 12 months +
                            if (txtBox.Tag != null && txtBox.Tag.ToString() == "8")
                            {
                                foreach (Control box in tab.Controls)
                                {
                                    if (box.GetType().ToString() == "System.Windows.Forms.TextBox")
                                    {
                                        // First box contains "txtBox1" the second must have the name "txtBox11"
                                        string name = txtBox.Name + txtBox.Name.Substring(txtBox.Name.Length - 1, 1);
                                        if (((TextBox)box).Name.Contains(name))
                                        {
                                            months = Convert.ToInt64(((TextBox)box).Text);
                                        }
                                    }
                                }

                                ChangeFinancialPreview(Convert.ToDecimal(value), "add", months);
                            }

                            // 9 - Multiply Dolar cash by 12 months -
                            if (txtBox.Tag != null && txtBox.Tag.ToString() == "9")
                            {
                                foreach (Control box in tab.Controls)
                                {
                                    if (box.GetType().ToString() == "System.Windows.Forms.TextBox")
                                    {
                                        // First box contains "txtBox1" the second must have the name "txtBox11"
                                        string name = txtBox.Name + txtBox.Name.Substring(txtBox.Name.Length - 1, 1);
                                        if (((TextBox)box).Name.Contains(name))
                                        {
                                            months = Convert.ToInt64(((TextBox)box).Text);
                                        }
                                    }
                                }

                                ChangeFinancialPreview(Convert.ToDecimal(value), "sub", months);
                            }
                        }
                    }
                }
            }
        }

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

                                if (txtbox.Tag != null && txtbox.Tag.ToString() == "10")
                                {
                                    txtbox.Text = "12";
                                }

                                if (txtbox.Tag != null && txtbox.Tag.ToString() == "11")
                                {
                                    txtbox.Text = "2";
                                }

                                if (txtbox.Name == "txtCurrency")
                                {
                                    txtbox.Text = "1";
                                }
                            }
                        }
                    }

                    yearPrev.Text = "000.000,00";
                    negPrev.Text = "000.000,00";
                    posPrev.Text = "000.000,00";
                    lblAnoReal.Text = "000.000,00";

                    lblDolar.BackColor = lblReal.BackColor = yearPrev.BackColor = lblAnoReal.BackColor = Color.Transparent;

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

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (loaded)
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

                File.WriteAllText(@loadedPath, save);
            }
            else
            {
                try
                {
                    if (locked == false)
                    {
                        DialogResult result;
                        string save = "";
                        string pathChoose = "";

                        result = savePath.ShowDialog();
                        pathChoose = savePath.FileName;
                        pathChoose += ".txt";

                        if (result == DialogResult.OK)
                        {
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

                            File.WriteAllText(@pathChoose, save);
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
            try
            {
                if (locked == false)
                {
                    DialogResult result;
                    string save = "";
                    string pathChoose = "";

                    result = savePath.ShowDialog();
                    pathChoose = savePath.FileName;

                    if (!pathChoose.Contains(".txt"))
                    {
                        pathChoose += ".txt";
                    }

                    if (result == DialogResult.OK)
                    {
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

                        File.WriteAllText(@pathChoose, save);
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
                    textBox1.Focus();
                }
            }
        }
        #endregion

        /*private void calendarStart_DateSelected(object sender, DateRangeEventArgs e)
        {
            tripDate = calendarStart.SelectionRange.Start;
        }*/
    }
}

