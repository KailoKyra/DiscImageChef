﻿// /***************************************************************************
// Aaru Data Preservation Suite
// ----------------------------------------------------------------------------
//
// Filename       : Identify.cs
// Author(s)      : Natalia Portillo <claunia@claunia.com>
//
// Component      : Disk image plugins.
//
// --[ Description ] ----------------------------------------------------------
//
//     Identifies Apple New Disk Image Format.
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
using Aaru.CommonTypes.Interfaces;
using Claunia.RsrcFork;

namespace Aaru.DiscImages
{
    public partial class Ndif
    {
        public bool Identify(IFilter imageFilter)
        {
            if(!imageFilter.HasResourceFork() ||
               imageFilter.GetResourceForkLength() == 0)
                return false;

            try
            {
                var rsrcFork = new ResourceFork(imageFilter.GetResourceForkStream());

                if(!rsrcFork.ContainsKey(NDIF_RESOURCE))
                    return false;

                Resource rsrc = rsrcFork.GetResource(NDIF_RESOURCE);

                if(rsrc.ContainsId(NDIF_RESOURCEID))
                    return true;
            }
            catch(InvalidCastException)
            {
                return false;
            }

            return false;
        }
    }
}