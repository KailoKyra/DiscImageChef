// /***************************************************************************
// Aaru Data Preservation Suite
// ----------------------------------------------------------------------------
//
// Filename       : Consts.cs
// Author(s)      : Natalia Portillo <claunia@claunia.com>
//
// Component      : Aaru Remote.
//
// --[ Description ] ----------------------------------------------------------
//
//     Constants for the Aaru Remote protocol.
//
// --[ License ] --------------------------------------------------------------
//
//     This library is free software; you can redistribute it and/or modify
//     it under the terms of the GNU Lesser General Public License as
//     published by the Free Software Foundation; either version 2.1 of the
//     License, or (at your option) any later version.
//
//     This library is distributed in the hope that it will be useful, but
//     WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
//     Lesser General Public License for more details.
//
//     You should have received a copy of the GNU Lesser General Public
//     License along with this library; if not, see <http://www.gnu.org/licenses/>.
//
// ----------------------------------------------------------------------------
// Copyright © 2011-2020 Natalia Portillo
// ****************************************************************************/

namespace Aaru.Devices.Remote
{
    public class Consts
    {
        public const uint RemoteId      = 0x52434944; // "DICR"
        public const uint PacketId      = 0x544B4350; // "PCKT"
        public const int  PacketVersion = 1;
        public const int  MaxProtocol   = 1;
    }
}