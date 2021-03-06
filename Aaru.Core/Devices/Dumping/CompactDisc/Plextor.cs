// /***************************************************************************
// Aaru Data Preservation Suite
// ----------------------------------------------------------------------------
//
// Filename       : Plextor.cs
// Author(s)      : Natalia Portillo <claunia@claunia.com>
//
// Component      : CompactDisc dumping.
//
// --[ Description ] ----------------------------------------------------------
//
//     Enables reading subchannel using Plextor vendor command.
//
// --[ License ] --------------------------------------------------------------
//
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as
//     published by the Free Software Foundation, either version 3 of the
//     License, or (at your option) any later version.
//
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
//
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
// ----------------------------------------------------------------------------
// Copyright © 2011-2020 Natalia Portillo
// ****************************************************************************/

using System;
using Aaru.Devices;

namespace Aaru.Core.Devices.Dumping
{
    partial class Dump
    {
        bool ReadPlextorWithSubchannel(out byte[] cmdBuf, out byte[] senseBuf, uint firstSectorToRead, uint blockSize,
                                       uint blocksToRead, PlextorSubchannel supportedPlextorSubchannel,
                                       out double cmdDuration)
        {
            bool sense;
            cmdBuf = null;

            if(supportedPlextorSubchannel == PlextorSubchannel.None)
            {
                sense = _dev.PlextorReadCdDa(out cmdBuf, out senseBuf, firstSectorToRead, blockSize, blocksToRead,
                                             supportedPlextorSubchannel, 0, out cmdDuration);

                if(!sense)
                    return false;

                // As a workaround for some firmware bugs, seek far away.
                _dev.PlextorReadCdDa(out _, out senseBuf, firstSectorToRead - 32, blockSize, blocksToRead,
                                     supportedPlextorSubchannel, 0, out _);

                sense = _dev.PlextorReadCdDa(out cmdBuf, out senseBuf, firstSectorToRead, blockSize, blocksToRead,
                                             supportedPlextorSubchannel, _dev.Timeout, out cmdDuration);

                return sense;
            }

            byte[] subBuf;

            uint subSize = supportedPlextorSubchannel == PlextorSubchannel.Q16 ? 16u : 96u;

            sense = _dev.PlextorReadCdDa(out byte[] dataBuf, out senseBuf, firstSectorToRead, 2352, blocksToRead,
                                         PlextorSubchannel.None, 0, out cmdDuration);

            if(!sense)
            {
                sense = _dev.PlextorReadCdDa(out subBuf, out senseBuf, firstSectorToRead, subSize, blocksToRead,
                                             supportedPlextorSubchannel, 0, out cmdDuration);

                if(!sense)
                {
                    cmdBuf = new byte[(2352 * blocksToRead) + (subSize * blocksToRead)];

                    for(int b = 0; b < blocksToRead; b++)
                    {
                        Array.Copy(dataBuf, 2352   * b, cmdBuf, (2352 + subSize) * b, 2352);
                        Array.Copy(subBuf, subSize * b, cmdBuf, ((2352 + subSize) * b) + 2352, subSize);
                    }

                    return false;
                }
            }

            // As a workaround for some firmware bugs, seek far away.
            _dev.PlextorReadCdDa(out _, out senseBuf, firstSectorToRead - 32, blockSize, blocksToRead,
                                 supportedPlextorSubchannel, 0, out _);

            sense = _dev.PlextorReadCdDa(out dataBuf, out senseBuf, firstSectorToRead, 2352, blocksToRead,
                                         PlextorSubchannel.None, 0, out cmdDuration);

            if(sense)
                return true;

            sense = _dev.PlextorReadCdDa(out subBuf, out senseBuf, firstSectorToRead, subSize, blocksToRead,
                                         supportedPlextorSubchannel, 0, out cmdDuration);

            if(sense)
                return true;

            cmdBuf = new byte[(2352 * blocksToRead) + (subSize * blocksToRead)];

            for(int b = 0; b < blocksToRead; b++)
            {
                Array.Copy(dataBuf, 2352   * b, cmdBuf, (2352 + subSize) * b, 2352);
                Array.Copy(subBuf, subSize * b, cmdBuf, ((2352 + subSize) * b) + 2352, subSize);
            }

            return false;
        }
    }
}