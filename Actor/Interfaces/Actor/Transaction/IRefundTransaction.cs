 
﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
﻿using Dotpay.Common.Enum;
﻿using Orleans;

namespace Dotpay.Actor.Interfaces
{
    /// <summary>
    /// Orleans grain communication interface RefundTransaction
    /// </summary>
    public interface IRefundTransaction : Orleans.IGrainWithGuidKey
    {
        Task Initiliaze(Guid sourceTransactionId, Guid accountId, RefundTransactionType refundTransactionType, CurrencyType currency, decimal amount);
        Task ConfirmRefundPreparation();
        Task Confirm();

        Task<RefundTransactionStatus> GetStatus();
    }
}
