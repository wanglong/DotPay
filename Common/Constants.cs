﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dotpay.Common
{
    public partial class Constants
    {

        /// <summary>current user key
        /// </summary>
        public const string CURRENT_USER_KEY = "__CURRENTUSER_ID_EMAIL";
        public const string CURRENT_USER_WAIT_VERIFY_TWO_FACTOR_KEY = "__CURRENTUSER_WAIT_VERIFY_TWO_FACTOR_ID_EMAIL";

        public const string StorageProviderName = "MongoDBStore";

        #region MQ Name

        public const string ExechangeSuffix = "Exchange";
        public const string QueueSuffix = "Queue";
        public const string DepositTransactionManagerMQName = "DepositTransactionManager";
        public const string TaobaoMQName = "Taobao";

        public const string TransferTransactionManagerMQName = "TransferTransactionManager";
        public const string TransferTransactionManagerRouteKey = "Inside";

        /*此处的Ripple相关的名称不可随意修改，这里的名称 在nodejs端也有使用*/
        /**/public const string TransferToRippleQueueName = "TransferToRipple";
        /**/public const string TransferToRippleRouteKey = "ToRipple";
        /*-----------------------------------------------------------------*/

        public const string RefundTransactionManagerMQName = "RefundTransactionManager";

        //此处的Ripple相关的名称不可随意修改，这里的名称 在nodejs端也有使用
        /**/public const string RippleTxResultMQName = "RippleTxResult";
        /**/public const string RippleValidatorMQName = "RippleValidator";                 //验证和获取last ledger index
        /**/public const string RippleLedgerIndexResultQueueName = "RippleGetLedgerIndex"; //获取last ledger index的Replay Queue 
        /**/public const string RippleValidateResultQueueName = "RippleValidator";         //tx验证消息Replay Queue
        /*-----------------------------------------------------------------*/

        public const string RippleToFIMQName = "RippleToFI";

        public const string RippleTrustLineMQName = "__RippleTrustLine_";


        public const string UserMQName = "UserMQ";
        public const string UserRouteKey = "User";
        public const string UserEmailRouteKey = "Email";
        public const string UserEmailMQName = "UserEmailMQ";

        #endregion
    }
}
