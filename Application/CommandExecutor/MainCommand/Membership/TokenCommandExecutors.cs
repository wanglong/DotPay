﻿using FC.Framework;
using FC.Framework.Repository;
using FC.Framework.Utilities;
using DotPay.Common;
using DotPay.MainDomain;
using DotPay.MainDomain.Exceptions;
using DotPay.MainDomain.Repository;
using DotPay.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotPay.Command.Executor
{
    public class TokenCommandsExecutors : ICommandExecutor<TokenUse>
    {
        public void Execute(TokenUse cmd)
        {
            Check.Argument.IsNotNull(cmd, "cmd");

            var token = IoC.Resolve<IRepository>().FindById<Token>(cmd.TokenID);

            token.MarkUsed();
        }
    }
}