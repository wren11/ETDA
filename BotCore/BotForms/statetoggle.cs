using System;
using System.Windows.Forms;
using BotCore.States;

namespace BotCore.BotForms
{
    public partial class statetoggle : UserControl
    {
        public Client client { get; set; }

        public BotInterface botinterface { get; set; }

        internal GameState state;

        public statetoggle()
        {
            InitializeComponent();
            this.Cursor = Cursors.Hand;
        }

        private void toolStripLabel1_Click(object sender, EventArgs e)
        {
            botinterface.MakeActiveState(this.toolStripLabel1.Text);
        }

        internal void SetInfo()
        {
            toolStripLabel1.Text = state.GetType().Name;
            toolStripLabel2.Text = state.Priority.ToString();
        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

    }
}
