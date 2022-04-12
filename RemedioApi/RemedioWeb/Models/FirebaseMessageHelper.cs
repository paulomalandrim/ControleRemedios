using FirebaseAdmin.Messaging;
using RemedioApi.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RemedioWeb.Models
{
    public class FirebaseMessageHelper
    {
        public static bool SendMessage(RemedioEntities context, string title, string message)
        {
            bool ret = false;

            try
            {
                if (context != null)
                {
                    List<string> destinationTokens = new List<string>();

                    var appTokens = context.tb_FcmToken.ToList();

                    if (appTokens != null)
                    {
                        foreach (var token in appTokens)
                        {
                            destinationTokens.Add(token.FcmToken);
                        }
                    }

                    if (destinationTokens.Count > 0)
                    {
                        var fcmMessage = new MulticastMessage()
                        {
                            Tokens = destinationTokens.Distinct().ToList(),

                            Notification = new Notification()
                            {
                                Title = title,
                                Body = message
                            }
                        };

                        var response = FirebaseMessaging.DefaultInstance.SendMulticastAsync(fcmMessage).Result;
                    }
                }

                ret = true;
            }
            catch (Exception ex)
            {
                ret = false;
            }

            return ret;
        }
    }
}