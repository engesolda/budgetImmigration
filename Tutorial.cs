using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BudgetImmigration
{
    public partial class Tutorial : Form
    {
        private int count = 1;

        public Tutorial()
        {
            InitializeComponent();
        }

        private void btnSkip_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            count++;

            if (count == 2)
            {
                btnPreview.Enabled = true;
            }
            if (count == 5)
            {
                btnNext.Text = "Finalizar";
            }
            if (count == 6)
            {
                this.Close();
            }

            ChangeImage();
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            count--;

            if (count == 1)
            {
                btnPreview.Enabled = false;
            }
            if (count == 5)
            {
                btnNext.Text = "Próximo";
            }

            ChangeImage();
        }

        private void ChangeImage ()
        {
            try
            {
                if (count == 1)
                {
                    picBoxTutorial.Image = BudgetImmigration.Properties.Resources.Tutorial1;
                }
                if (count == 2)
                {
                    picBoxTutorial.Image = BudgetImmigration.Properties.Resources.Tutorial2;
                }
                if (count == 3)
                {
                    picBoxTutorial.Image = BudgetImmigration.Properties.Resources.Tutorial3;
                }
                if (count == 4)
                {
                    picBoxTutorial.Image = BudgetImmigration.Properties.Resources.Tutorial4;
                }
                if (count == 5)
                {
                    picBoxTutorial.Image = BudgetImmigration.Properties.Resources.Tutorial5;
                }
                if (count == 6)
                {
                    picBoxTutorial.Image = BudgetImmigration.Properties.Resources.Tutorial6;
                    btnNext.Text = "Finalizar";
                }

                picBoxTutorial.Invalidate();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar foto do tutorial.");
                this.Close();
            }
        }
    }
}
