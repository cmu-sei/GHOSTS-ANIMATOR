// Copyright 2020 Carnegie Mellon University. All Rights Reserved. See LICENSE.md file for terms.

using System.IO;
using PgpCore;

namespace Ghosts.Animator.Services
{
    public static class PgpService
    {
        public static PgpProfile Generate(string email, string password = null)
        {
            if (password == null)
                password = Internet.GetPassword();
            
            var p = new PgpProfile();
            
            using (var pgp = new PGP())
            {
                const string publicFile = "public.asc";
                const string privateFile = "private.asc";

                // Generate keys
                pgp.GenerateKey(publicFile, privateFile, email, password);

                p.Email = email;
                p.Password = password;
                p.PublicKey = File.ReadAllText(publicFile);
                p.PrivateKey = File.ReadAllText(privateFile);
                
                File.Delete(publicFile);
                File.Delete(privateFile);
            }
            return p;
        }

        public class PgpProfile
        {
            public string Email { get; set; }
            public string Password { get; set; }
            public string PublicKey { get; set; }
            public string PrivateKey { get; set; }
        }
    }
}