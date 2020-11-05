/*
GHOSTS ANIMATOR
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon® and CERT® are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0930
*/

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