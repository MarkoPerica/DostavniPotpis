using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DostavniPotpis.Messages
{
    public class SendBarcodeDecode : ValueChangedMessage<string>
    {
        public SendBarcodeDecode(string value) : base(value)
        {
        }
    }
}
