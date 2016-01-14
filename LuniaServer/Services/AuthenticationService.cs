using System;
using System.Collections.Generic;
using System.Linq;
using LuniaAssembly;
using LuniaAssembly.Packet;
using LuniaAssembly.States;
using LuniaServer.Persistance;

namespace LuniaServer
{
    public class AuthenticationService
    {

        private List<User> Users;
        private Server server;

        public AuthenticationService(Server server)
        {
            this.server = server;
        }

        public AuthenticationService()
        {
            Users = new List<User> { new User(new Guid("5B01E45D-D295-42B8-A8BF-28064E01F080"), "Digot", "7388fda378261e17c01d4a7b50604257046f8bb3") };
        }


        public void ProcessAuthentication(Connection connection, LCAuthentication authentication)
        {
            //Check the salt
            if (!validateSalt(authentication.Salt))
            {
                //Probably a custom made connection, drop it and quit authentication
                connection.Socket.Disconnect(false);
                Console.WriteLine(authentication.Username + " failed authentication with reason: WRONG SALT");
                return;
            }

            //Prepare the response
            LCAuthenticationResponse response = new LCAuthenticationResponse();

            //Check the username
            if (!checkUsername(authentication.Username))
            {
                //Username is not existing in the database, send this result to the user and quit authentication
                response.Result = AuthenticationResult.BAD_USERNAME;
                connection.Send(response);
                Console.WriteLine(authentication.Username + " failed authentication with reason: BAD USERNAME");
                return;
            }

            //Check the username with the password
            if (!checkPassword(authentication.Username, authentication.Password))
            {
                //Password is not correct for this username, send this result to the user and quit authentication
                response.Result = AuthenticationResult.BAD_PASSWORD;
                connection.Send(response);
                Console.WriteLine(authentication.Username + " failed authentication with reason: BAD PASSWORD");
                return;
            }

            //Authentication succeeded
            response.Result = AuthenticationResult.SUCCESS;
            connection.Send(response);
            Console.WriteLine(authentication.Username + " successfully authenticated");

            //Send state switch
            LCStateSwitch stateSwitch = new LCStateSwitch { GameState = GameState.CHARACTER_SELECTION };
            connection.Send(stateSwitch);
            Console.WriteLine("Sent game state switch to " + stateSwitch.GameState + " - User: " +
                              authentication.Username);

            //Send character list
            server.CharacterService.ListCharacters(connection );
            Console.WriteLine("Sent character list");
        }

        private bool checkPassword(string username, string password)
        {
            return checkUsername(username) && Users.Any(x => x.HashedPassword.Equals(password, StringComparison.CurrentCultureIgnoreCase));
        }

        private bool checkUsername(string username)
        {
            return Users.Any(x => x.Name.Equals(username, StringComparison.CurrentCultureIgnoreCase));
        }

        private bool validateSalt(string salt)
        {
            return (Encrypter.GetHashed(TCPServer.IP) == salt);
        }

    }
}