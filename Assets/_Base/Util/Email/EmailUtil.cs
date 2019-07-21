using System;
using UnityEngine;

namespace BaseFramework
{
    public class EmailUtil
    {
        public static void Send(string toEmail, string subject = "", string body = "")
        {
            string uriString = $"mailto:{toEmail}?subject={subject}&body={body}";
            Uri uri = new Uri(uriString);
            Application.OpenURL(uri.AbsoluteUri);
        }
    }
}