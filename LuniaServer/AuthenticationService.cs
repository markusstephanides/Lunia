using System;
using LuniaAssembly;
using LuniaAssembly.Packet;

namespace LuniaServer
{
    public class AuthenticationService
    {

    

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
        }

        private bool checkPassword(string username, string password)
        {
            return string.Equals(username, "Digot", StringComparison.CurrentCultureIgnoreCase) &&  string.Equals(password, Encrypter.GetHashed(("test")));
        }

        private bool checkUsername(string username)
        {
            return string.Equals(username, "Digot", StringComparison.CurrentCultureIgnoreCase);
        }

        private bool validateSalt(string salt)
        {
            return (Encrypter.GetHashed(TCPServer.IP) == salt);
        }

    }
}