using BotCore.States.BotStates;
using System;
using System.Windows.Forms;

namespace BotCore.States
{
    [Serializable]
    public partial class StateSettings : UserControl
    {
        public delegate void SettingsUpdated(GameState state);

        public event SettingsUpdated OnSettingsUpdated = null;
        private GameState State { get; set; }

        private bool Running = false;

        public StateSettings(GameState state)
        {
            InitializeComponent();
            State = state;

            StateAttribute attributes =
                (StateAttribute)Attribute.GetCustomAttribute(state.GetType(), typeof(StateAttribute));
            StateMetaInfo meta =
                (StateMetaInfo)Attribute.GetCustomAttribute(state.GetType(), typeof(StateMetaInfo));

            label2.Text = string.Format("Developed by {0}", attributes.Author);
            if (meta != null && meta.Version != null)
                label2.Text += "\nVersion: " + meta.Version + "\nLast Updated: " + meta.DateUpdated;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (State != null)
            {
                State.Run(TimeSpan.FromSeconds(1));
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Running = !Running;

            if (Running == false)
                State.Client.CleanUpMememory();

            if (State != null)
                State.Enabled = Running;

                OnSettingsUpdated?.Invoke(State);
        }

        private void StateSettings_Load(object sender, EventArgs e)
        {
            statename.Text = State.GetType().Name;
            numericUpDown1.Value = State.Priority;

            OnSettingsUpdated?.Invoke(State);

            propertyGrid1.SelectedObject = null;
            propertyGrid1.SelectedObject = State;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (State != null)
            {
                State.Priority = (int)numericUpDown1.Value;
                OnSettingsUpdated?.Invoke(State);
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            State = (GameState)propertyGrid1.SelectedObject;
        }
    }
}