// SPDX-FileCopyrightText: 2022 Demerzel Solutions Limited
// SPDX-License-Identifier: LGPL-3.0-only

using Nethermind.Core;

namespace Nethermind.TxPool
{
    public interface ITxPoolInfoProvider
    {
        TxPoolInfo GetInfo();
    }
}
