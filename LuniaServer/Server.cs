using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows.Forms;
using LuniaAssembly;
using LuniaAssembly.Packet;

namespace LuniaServer
{
    public class Server
    {
        private ServerGUI serverGui;
        private HashSet<IPlayer> players;
        private SimpleServer simpleServer;

        public AuthenticationService AuthenticationService { get; }
        public CharacterService CharacterService { get; }

        public Server(StartupSettings settings)
        {
            bootstrap(settings);

            AuthenticationService = new AuthenticationService(this);
            CharacterService = new CharacterService();
        }

        private void bootstrap(StartupSettings settings)
        {
            if (!settings.NoGUI)
            {
                serverGui = new ServerGUI();
                Application.EnableVisualStyles();

                Task mytask = Task.Run(() =>
                {
                    ServerGUI form = new ServerGUI();
                    form.ShowDialog();
                });
            }
          
            simpleServer = new SimpleServer(settings.Port);
            
            simpleServer.RegisterHandler(1, handleAuthentication);
        }

        private void handleAuthentication(Connection connection, IPacket packet)
        {
            LCAuthentication authentication = (LCAuthentication) packet;
            AuthenticationService.ProcessAuthentication(connection, authentication);
        }
    }
}