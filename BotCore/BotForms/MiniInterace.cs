using BotCore.Shared.Helpers;
using System;
using System.Windows.Forms;

namespace BotCore
{
    public partial class MiniInterace : Form
    {
        public Client client { get; set; }
        public MiniInterace(Client c)
        {
            InitializeComponent();
            this.client = c;
            this.FormClosing += MiniInterace_FormClosing;
            this.VisibleChanged += MiniInterace_VisibleChanged;
            button1.Text = client.Paused ? "Paused." : "Running!";
        }

        private void MiniInterace_VisibleChanged(object sender, EventArgs e)
        {
            this.Text = client.Attributes.PlayerName + " (" + client.Attributes.Serial + ")";
        }

        private void MiniInterace_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            if (Visible)
            {
                Hide();
                client.BotForm.Show();
            }
        }

        internal void UpdateStatistics()
        {
            if (client.Attributes.HP == 0 && client.Attributes.MP == 0)
                return;

            label2.Text = client.Attributes.HP.ToString();
            label3.Text = client.Attributes.MP.ToString();

            label4.Text = string.Format("Targeted Monsters: {0}", client.ObjectSearcher.TargetedMonsters.Count);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            client.Paused = !client.Paused;

            if (client.Paused)
            {
                button1.Text = "Paused.";
            }
            else
            {
                button1.Text = "Running!";
            }
        }
    }
}
