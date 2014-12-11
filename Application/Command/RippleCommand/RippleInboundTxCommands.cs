﻿using DotPay.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotPay.RippleCommand
{
    public class CreateInboundTx : FC.Framework.Command
    {
        public CreateInboundTx(string txid, int destinationTag, decimal amount)
        {
            this.TxID = txid;
            this.DestinationTag = destinationTag;
            this.Amount = amount;
        }
        public string TxID { get; protected set; }
        public int DestinationTag { get; protected set; }
        public decimal Amount { get; protected set; }
    }

    public class CreateThirdPartyPaymentInboundTx : FC.Framework.Command
    {
        public CreateThirdPartyPaymentInboundTx(PayWay payway, string destination)
        {
            this.PayWay = payway;
            this.Destination = destination;
        }
        public PayWay PayWay { get; protected set; }
        public string Destination { get; protected set; }
        public int Result { get; set; }
    }

    public class CompleteThirdPartyPaymentInboundTx : FC.Framework.Command
    {
        public CompleteThirdPartyPaymentInboundTx(PayWay payway, string txid, int destinationtag, decimal amount)
        {
            this.PayWay = payway;
            this.TxId = txid;
            this.DestinationTag = destinationtag;
            this.Amount = amount;
        }
        public PayWay PayWay { get; protected set; }
        public string TxId { get; protected set; }
        public int DestinationTag { get; set; }
        public decimal Amount { get; set; }
    }
}
