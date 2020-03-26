// /***************************************************************************
// Aaru Data Preservation Suite
// ----------------------------------------------------------------------------
//
// Filename       : Constants.cs
// Author(s)      : Maxime Croizer <maximecroizer@gmail.com>
//
// Component      : Tape image plugins.
//
// --[ Description ] ----------------------------------------------------------
//
//     Contains constants for CDT/TZX tape images.
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

namespace Aaru.DiscImages
{
    public partial class TZX
    {
        /// <summary>Identifier for CPC CDT disk images, always "ZXTape!"</summary>
        readonly byte[] tzxMagic = 
        {
            0x5a, 0x58, 0x54, 0x61, 0x70, 0x65, 0x21, 0x1A
        };

        readonly uint z80TStatePerSecond = 3500000;

        // Other constan
    }
}