// /***************************************************************************
// Aaru Data Preservation Suite
// ----------------------------------------------------------------------------
//
// Filename       : Read.cs
// Author(s)      : Natalia Portillo <claunia@claunia.com>
//
// Component      : Disk image plugins.
//
// --[ Description ] ----------------------------------------------------------
//
//     Reads CPCEMU disk images.
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Aaru.Checksums;
using Aaru.CommonTypes;
using Aaru.CommonTypes.Enums;
using Aaru.CommonTypes.Exceptions;
using Aaru.CommonTypes.Interfaces;
using Aaru.Console;
using Aaru.Decoders.Floppy;
using Aaru.Helpers;

namespace Aaru.DiscImages
{
    public partial class TZX
    {
        public bool Open(IFilter imageFilter)
        {
            Stream stream = imageFilter.GetDataForkStream();
            stream.Seek(0, SeekOrigin.Begin);

            if(stream.Length < 10)
                return false;

            byte[] headerB = new byte[10];
            stream.Read(headerB, 0, 10);
            TZXHeader header = Marshal.ByteArrayToStructureLittleEndian<TZXHeader>(headerB);

            if(!header.magic.SequenceEqual(tzxMagic))
                return false;

            AaruConsole.DebugWriteLine("TZX plugin", "header.magic = \"{0}\"",
                                       StringHandlers.CToString(header.magic));

            AaruConsole.DebugWriteLine("TZX plugin", "header.version = \"{0}.{1}\"",
                                       header.versionMajor, header.versionMinor);

            imageInfo.Application          = "";
            imageInfo.CreationTime         = imageFilter.GetCreationTime();
            imageInfo.LastModificationTime = imageFilter.GetLastWriteTime();
            imageInfo.MediaTitle           = Path.GetFileNameWithoutExtension(imageFilter.GetFilename());
            imageInfo.XmlMediaType         = XmlMediaType.AudioMedia;
            imageInfo.MediaType            = MediaType.CompactCassette;

            return true;
        }

    }
}