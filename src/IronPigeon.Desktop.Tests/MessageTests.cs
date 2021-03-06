﻿// Copyright (c) Andrew Arnott. All rights reserved.
// Licensed under the Microsoft Reciprocal License (Ms-RL) license. See LICENSE file in the project root for full license information.

namespace IronPigeon.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    public class MessageTests
    {
        [Fact]
        public void CtorInvalidArgs()
        {
            Assert.Throws<ArgumentNullException>(() => new Payload(null, Valid.ContentType));
            Assert.Throws<ArgumentNullException>(() => new Payload(Valid.MessageContent, null));
            Assert.Throws<ArgumentException>(() => new Payload(Valid.MessageContent, string.Empty));
        }

        [Fact]
        public void Ctor()
        {
            var message = new Payload(Valid.MessageContent, Valid.ContentType);
            Assert.Same(Valid.MessageContent, message.Content);
            Assert.Equal(Valid.ContentType, message.ContentType);
        }
    }
}
