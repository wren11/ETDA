using System;
using System.Collections.Generic;
using System.Threading;
using BotCore;
using BotCore.Components;
using System.Windows.Forms;
using System.IO;

//public release......

namespace Bot
{
    class Program
    {
        //60 Frames per second!
        static TimeSpan UpdateSpan { get; set; }

        //Track Frame Updates
        static DateTime LastUpdate { get; set; }

        //List of Updateable components, Add more here if needed.
        static List<UpdateableComponent> Components = new List<UpdateableComponent>();

        static MainForm ParentForm = new MainForm();

        static Thread UpdatingThread = null;

        [STAThread]
        static void Main(string[] args)
        {
            //set parent
            Collections.ParentForm = ParentForm;

            //Set a tick rate of 60 frames per second.
            UpdateSpan = TimeSpan.FromSeconds(1.0 / 60.0);

            //Initialize Components
            SetupComponents();

            //Update Frame
            UpdatingThread = new Thread(new ThreadStart(DoUpdate));
            UpdatingThread.IsBackground = true;
            UpdatingThread.Start();

            //this simply runs the MainForm Thread
            Application.Run(ParentForm);
        }

        //This Function Sets up Components used for this bot.
        private static void SetupComponents()
        {
            //a process monitor Component, It will listen for new DA Windows,
            //and attach ETDA.DLL to them, and remove ETDA.dll when client is closed.
            var monitor = new ProcessMonitor();
            monitor.Attached += monitor_Attached;
            monitor.Removed += monitor_Removed;

            //Add this to our component list, so it will get updated in the main frame.
            Components.Add(monitor);
        }

        //DA process was removed, unload all necessary resources.
        static void monitor_Removed(object sender, EventArgs e)
        {
            var client = Collections.AttachedClients[(int)sender];
            client.CleanUpMememory();
            client.DestroyResources();
            Collections.AttachedClients.Remove((int)sender);
        }

        //DA Process was removed.
        static void monitor_Attached(object sender, EventArgs e)
        {
            //create a new client class for this DA Process
            var client = new Client();

            //prepare ETDA.dll and inject it into the process.
            client.InitializeMemory(
                System.Diagnostics.Process.GetProcessById((int)sender), 
                Path.Combine(Environment.CurrentDirectory, "EtDA.dll"));

            //Add to our Global collections dictionary.
            Collections.AttachedClients[(int)sender] = client;

            ParentForm.Invoke((MethodInvoker)delegate ()
            {
                //invoke OnAttached, so signal creation of Packet Handlers.
                client.OnAttached();
            });
        }

        //this is updated 60 frames per second.
        //It's job is to update and elapsed components.
        static void DoUpdate()
        {
            while (true)
            {
                var delta = (DateTime.Now - LastUpdate);
                try
                {
                    Update(delta);
                }
                catch (Exception)
                {
                    continue;
                }
                finally
                {
                    LastUpdate = DateTime.Now;
                    Thread.Sleep(UpdateSpan);
                }
            }
        }

        static void Update(TimeSpan elapsedTime)
        {
            //Update all components.
            lock (Components)
            {
                Components.ForEach(i => i.Update(elapsedTime));
            }

            //Update all attached clients in our collections dictionary, this will allow
            //any updateable components inside client to also update accordinaly to the elapsed frame.

            //copy memory here is deliberate!
            var copy = new List<Client>();
            lock (Collections.AttachedClients)
                copy = new List<Client>(Collections.AttachedClients.Values);

            var clients = copy.ToArray();
            foreach (var c in clients)
                c.Update(elapsedTime);
        }
    }
}
