using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CommonScheduler.Authentication.PasswordPolicy
{
    class PasswordAdvisor
    {
        public static PasswordScore CheckStrength(string password)
        {
            int score = 1;

            if (password.Length < 1)
                return PasswordScore.Blank;
            if (password.Length < 4)
                return PasswordScore.VeryWeak;

            if (password.Length >= 6)
                score++;
            //if (password.Length >= 12)
            //    score++;
            if (Regex.Match(password, @"\d+", RegexOptions.None).Success)
                score++;
            //if (Regex.Match(password, @"[a-z]+", RegexOptions.None).Success &&
            //  Regex.Match(password, @"[A-Z]+", RegexOptions.None).Success)
            //    score++;
            if (Regex.Match(password, @".[!,@,#,$,%,^,&,*,?,_,~,-,£,(,)]+", RegexOptions.None).Success)
                score++;

            return (PasswordScore)score;
        }
    }
}
