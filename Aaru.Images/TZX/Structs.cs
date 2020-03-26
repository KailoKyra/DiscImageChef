// /***************************************************************************
// Aaru Data Preservation Suite
// ----------------------------------------------------------------------------
//
// Filename       : Structs.cs
// Author(s)      : Natalia Portillo <claunia@claunia.com>
//
// Component      : Disk image plugins.
//
// --[ Description ] ----------------------------------------------------------
//
//     Contains structures for TZX tape images.
//     https://www.worldofspectrum.org/TZXformat.html
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

using System.Runtime.InteropServices;
using Aaru.Decoders.Floppy;

namespace Aaru.DiscImages
{
    public partial class TZX
    {

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct TZXHeader
        {
            /// <summary>TZX Signature (always ZXTape!)
            /// The file is identified with the first 8 bytes being 'ZXTape!' plus 
            /// the 'end of file' byte 26 (1A hex). This is followed by two bytes 
            /// containing the major and minor version numbers. Then the main body of the file follows.
            /// It consists of a mixture of blocks, each identified by an ID byte.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public readonly byte[] magic;
            /// <summary>TZX major revision number</summary>
            public readonly byte versionMajor;
            /// <summary>TZX minor revision number</summary>
            public readonly byte versionMinor;
        }

        /// <summary>ID 10 - Standard Speed Data Block
        /// This block must be replayed with the standard Spectrum ROM timing values - see the values in curly brackets in block ID 11. The pilot tone consists in 8063 pulses if the first data byte (flag byte) is < 128, 3223 otherwise. This block can be used for the ROM loading routines AND for custom loading routines that use the same timings as ROM ones do.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct TZXStandardSpeedDataBlockHeader
        {
            /// <summary>Duration of the pause after this block (ms) </summary>
            public readonly ushort pauseAfterBlock;
            /// <summary>Size of the data to follow (in bytes)</summary>
            public readonly ushort dataLength;
        }
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct TZXStandardSpeedDataBlock
        {
            /// <summary>Block Header </summary>
            public readonly TZXStandardSpeedDataBlockHeader header;
            /// <summary>Data as in .TAP files</summary>
            public readonly byte[] data;
        }

        /// <summary>ID 11 - Turbo Speed Data Block
        /// This block is very similar to the normal TAP block but with some additional info on the timings and other important differences. The same tape encoding is used as for the standard speed data block. If a block should use some non-standard sync or pilot tones (i.e. all sorts of protection schemes) then use the next three blocks to describe it.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct TZXTurboSpeedDataBlockHeader
        {
            /// <summary>Lenght of PILOT pulse</summary>
            public readonly ushort pilotPulseLength;
            /// <summary>Length of SYNC first pulse</summary>
            public readonly ushort firstSyncPulseLength;
            /// <summary>Length of SYNC second pulse</summary>
            public readonly ushort secondSyncPulseLength;
            /// <summary>Length of ZERO bit pulse</summary>
            public readonly ushort zeroPulseLength;
            /// <summary>Length of ONE bit pulse</summary>
            public readonly ushort onePulseLenght;
            /// <summary>Length of PILOT tone (in number of pulses)</summary>
            public readonly ushort pilotToneLength;
            /// <summary>Used bits in the last byte (other bits should be 0): 
            /// Eg: if this is 6, then the bits used in the last byte are:
            /// xxxxxx00, where MSB is the leftmost bit</summary>
            public readonly byte usedBitsInTheLastByte;
            /// <summary>Pause after this block (in ms)</summary>
            public readonly ushort pauseDurationAfterThisBlock;
            /// <summary>Length of the data to follow</summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public readonly byte[] dataLength;
        }
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct TZXTurboSpeedDataBlock
        {
            /// <summary>Block Header </summary>
            public readonly TZXTurboSpeedDataBlockHeader header;
            /// <summary>Data as in .TAP files </summary>
            public readonly byte[] data;
        }


        /// <summary>ID 12 - Pure Tone
        /// This will produce a tone which is basically the same as the pilot tone in the ID 10, ID 11 blocks. You can define how long the pulse is and how many pulses are in the tone.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct TZXPureToneBlock
        {
            /// <summary>Length of one pulse in T-states</summary>
            public readonly ushort lenghtOfPulseInTStates;
            /// <summary>Number of pulses in tone </summary>
            public readonly ushort numberOfPulses;
        }

        /// <summary>ID 13 - Pulse sequence
        /// This will produce N pulses, each having its own timing. Up to 255 pulses can be stored in this block; this is useful for non-standard sync tones used by some protection schemes.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct TZXPulseSequenceBlockHeader
        {
            /// <summary>Number of Pulses</summary>
            public readonly ushort numberOfPulses;
        }
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct TZXPulseSequenceBlock
        {
            /// <summary>Number of Pulses</summary>
            public readonly TZXPulseSequenceBlockHeader header;
            /// <summary>Pulses' lengths</summary>
            public readonly ushort[] pulsesLengths;
        }


        /// <summary>ID 14 - Pure Data Block
        /// This is the same as in the turbo loading data block, except that it has no pilot or sync pulses.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct TZXPureDataBlockHeader
        {
            /// <summary>Length of ZERO bit pulse</summary>
            public readonly ushort zeroBitPulseLength;
            /// <summary>Length of ONE bit pulse</summary>
            public readonly ushort oneBitPulseLength;
            /// <summary>Used bits in the last byte (other bits should be 0): 
            /// Eg: if this is 6, then the bits used in the last byte are:
            /// xxxxxx00, where MSB is the leftmost bit</summary>
            public readonly byte usedBitsInTheLastByte;
            /// <summary>Pause after this block (in ms)</summary>
            public readonly ushort pauseDurationAfterThisBlock;
            /// <summary>Length of the data to follow</summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public readonly byte[] dataLength;
        }
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct TZXPureDataBlock
        {
            /// <summary>Block Header </summary>
            public readonly TZXPureDataBlockHeader header;
            /// <summary>Data as in .TAP files </summary>
            public readonly byte[] data;
        }


        /// <summary>ID 15 - Direct Recording
        /// This block is used for tapes which have some parts in a format such that the turbo loader block cannot be used. This is not like a VOC file, since the information is much more compact. Each sample value is represented by one bit only (0 for low, 1 for high) which means that the block will be at most 1/8 the size of the equivalent VOC.
        /// The preferred sampling frequencies are 22050 or 44100 Hz(158 or 79 T-states/sample). Please, if you can, don't use other sampling frequencies.
        /// Please use this block only if you cannot use any other block.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct TZXDirectRecordingBlockHeader
        {
            /// <summary>Number of T-states per sample (bit of data) </summary>
            public readonly ushort tStatesPerSample;
            /// <summary> Pause duration after this block in ms </summary>
            public readonly ushort pauseDurationAfterThisBlock;
            /// <summary> Used bits (samples) in last byte of data (1-8).
            /// 	Used bits (samples) in last byte of data (1-8)
            /// (e.g. if this is 2, only first two samples of the last byte will be played)
            /// </summary>
            public readonly byte usedBitsInLastByteOfData;
            /// <summary>Length of samples' data</summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public readonly byte[] lengthOfSamplesData;
            /// <summary> </summary>
        }
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct TZXDirectRecordingBlock
        {
            /// <summary>Block Header </summary>
            public readonly TZXDirectRecordingBlockHeader header;
            /// <summary>Samples data. Each bit represents a state on the EAR port (i.e. one sample).
            /// MSb is played first</summary>
            public readonly byte[] samplesData;
        }

        /// <summary>ID 18 - CSW Recording
        /// This block contains a sequence of raw pulses encoded in CSW format v2 (Compressed Square Wave).
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct TZXCSWRecordingBlockHeader
        {
            /// <summary> Block length (without these 4 bytes) </summary>
            public readonly uint blocklength;
            /// <summary> Pause duration after this block (in ms) </summary>
            public readonly ushort pauseDurationAfterThisBlock;
            /// <summary> Sampling rate </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public readonly byte[] samplingRate;
            /// <summary> Compression type (0x01:RLE, 0x02 Z-RLE) </summary>
            public readonly byte compressionType;
            /// <summary> Number of stored pulses (after decompression, for validation purposes) </summary>
            public readonly uint numberOfStoredPulses;
        }
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct TZXCSWRecordingBlock
        {
            /// <summary>Block Header </summary>
            public readonly TZXCSWRecordingBlockHeader header;
            /// <summary>CSW data, encoded according to the CSW file format specification </summary>
            public readonly byte[] data;
        }


        /// <summary>ID 19 - Generalized Data Block
        /// This block has been specifically developed to represent an extremely wide range of data encoding techniques.
        ///The basic idea is that each loading component(pilot tone, sync pulses, data) is associated to a specific sequence of pulses, where each sequence(wave) can contain a different number of pulses from the others.In this way we can have a situation where bit 0 is represented with 4 pulses and bit 1 with 8 pulses.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct TZXGeneralizedDataBlockHeader
        {
            /// <summary> Block length (without these 4 bytes) </summary>
            public readonly uint blocklength;
            /// <summary> Pause duration after this block (in ms) </summary>
            public readonly ushort pauseDurationAfterThisBlock;
            /// <summary> Total number of symbols in pilot/sync block (can be 0) </summary>
            public readonly uint totalNumberOfSymbols;
            /// <summary> Maximum number of pulses per pilot/sync symbol <summary>
            public readonly byte maxNbOfPulsesPerPilotOrSyncSymbol;
            /// <summary> Number of pilot/sync symbols in the alphabet table (0=256) </summary>
            public readonly byte nbOfPulsesOrSyncSymbolsInAlphabetTable;
            /// <summary> Total number of symbols in data stream (can be 0) </summary>
            public readonly uint nbOfSymbolsInDataStream;
            /// <summary> Maximum number of pulses per data symbol </summary>
            public readonly byte maxPulsesPerDataSymbol;
            /// <summary> Number of data symbols in the alphabet table (0=256) </summary>
            public readonly byte nbDataSymbolsInAlphabetTable;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct TZXGeneralizedDataBlock
        {
            /// <summary>Block Header </summary>
            public readonly byte TZXGeneralizedDataBlockHeader;
            /// <summary>Pilot and sync symbols definition table (This field is present only if totalNumberOfSymbols>0) </summary>
            public readonly SymDef[/*nbOfPulsesOrSyncSymbolsInAlphabetTable*/] pilotAndSyncDefTable;
            /// <summary> Pilot and sync data stream (This field is present only if totalNumberOfSymbols>0)  </summary>
            public readonly PRLE[/*totalNumberOfSymbols*/] pilotAndSyncDataStream;
            /// <summary> Data symbols definition table (This field is present only if TOTD>0)  </summary>
            public readonly SymDef[/*nbDataSymbolsInAlphabetTable*/] dataSymbolsDefinitionTable;
            /// <summary> Data Stream (This field is present only if TOTD > 0)  </summary>
            public readonly byte[] dataStream;
        }

        /// <summary>SYMDEF structure format
        ///The alphabet is stored using a table where each symbol is a row of pulses.The number of columns (i.e.pulses) of the table is the length of the longest sequence amongst all (MAXP= NPP or NPD, for pilot/sync or data blocks respectively); shorter waves are terminated by a zero-length pulse in the sequence.
        ///
        ///Any number of data symbols is allowed, so we can have more than two distinct waves; for example, imagine a loader which writes two bits at a time by encoding them with four distinct pulse lengths: this loader would have an alphabet of four symbols, each associated to a specific sequence of pulses (wave)        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct SymDef
        {
            /// <summary> Symbol Flag
            /// b0-b1 : starting symbol parity
            ///     00: opposite to the current level (make an edge, as usual) - default
            ///     01: same as the current level (no edge - prolongs the previous pulse)
            ///     10: force low level
            ///     11: force high level
            /// </summary>
            public readonly byte symbolFlags;
            ///<summary> Array of Pulse lengths </summary>
            public readonly ushort[] pulsesLengths;
        }

        /// <summary>PRLE structure format
        /// Most commonly, pilot and sync are repetitions of the same pulse, thus they are represented using a very simple RLE encoding structure which stores the symbol and the number of times it must be repeated.
        ///
        ///Each symbol in the data stream is represented by a string of NB bits of the block data, where NB = ceiling(Log2(ASD)). Thus the length of the whole data stream in bits is NB* TOTD, or in bytes DS = ceil(NB * TOTD / 8).
        ///
        /// </summary> 
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct PRLE
        {
            /// <summary> Symbol to be represented </summary>
            public readonly byte symbol;
            /// <summary> Number of repetitions </summary>
            public readonly short nbRepetitions;
        }


        /// <summary>ID 20 - Pause (silence) or 'Stop the Tape' command
        ///This will make a silence(low amplitude level (0)) for a given time in milliseconds.If the value is 0 then the emulator or utility should(in effect) STOP THE TAPE, i.e.should not continue loading until the user or emulator requests it.
        /// </summary> 
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct TZXPauseOrStopTheTapeBlock
        {
            /// <summary> Pause duration (ms) </summary>
            public readonly ushort pauseDuration;
        }

        /// <summary> ID 21 - Group Start 
        /// This block marks the start of a group of blocks which are to be treated as one single (composite) block. This is very handy for tapes that use lots of subblocks like Bleepload (which may well have over 160 custom loading blocks). You can also give the group a name (example 'Bleepload Block 1').
        ///
        ///For each group start block, there must be a group end block.Nesting of groups is not allowed.
        ///
        /// 
        /// </summary> 
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct TZXGroupStartBlockHeader
        {
            /// <summary> Length of the group name string) </summary>
            public readonly byte groupNameLength;
        }
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct TZXGroupStartBlock
        {
            /// <summary> Block Header </summary>
            public readonly TZXGroupStartBlockHeader header;
            /// <summary>Group name in ASCII format (please keep it under 30 characters long)  </summary>
            public readonly byte[] groupName;
        }

        /// <summary> ID 22 - Group End
        /// This indicates the end of a group. This block has no body. </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct TZXGroupEndBlock
        {
        }

        /// <summary> ID 23 - Jump to block
        /// This block will enable you to jump from one block to another within the file. The value is a signed short word (usually 'signed short' in C); Some examples:
        ///     Jump 0 = 'Loop Forever' - this should never happen
        ///     Jump 1 = 'Go to the next block' - it is like NOP in assembler ;)
        ///     Jump 2 = 'Skip one block'
        ///     Jump -1 = 'Go to the previous block'
        /// All blocks are included in the block count!
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct TZXJumpToBlock
        {
            /// <summary> Relative jump value </summary>
            public readonly short relativeJump;
        }

        /// <summary> ID 24 - Loop start
        /// If you have a sequence of identical blocks, or of identical groups of blocks, you can use this block to tell how many times they should be repeated. This block is the same as the FOR statement in BASIC.
        ///
        ///     For simplicity reasons don't nest loop blocks!
        ///
        /// </summary>
        /// 
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct TZXLoopStartBlock
        {
            /// <summary> Number of repetitions (greater than 1) </summary>
            public readonly ushort loopCount;
        }

        /// <summary> ID 25 - Loop End
        /// This is the same as BASIC's NEXT statement. It means that the utility should jump back to the start of the loop if it hasn't been run for the specified number of times.
        ///
        ///This block has no body
        ///
        /// </summary>
        /// 
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct TZXLoopEndBlock
        {
        }

        /// <summary> ID 26 - Call sequence
        /// This block is an analogue of the CALL Subroutine statement. It basically executes a sequence of blocks that are somewhere else and then goes back to the next block. Because more than one call can be normally used you can include a list of sequences to be called. The 'nesting' of call blocks is also not allowed for the simplicity reasons. You can, of course, use the CALL blocks in the LOOP sequences and vice versa. The value is relative for the obvious reasons - so that you can add some blocks in the beginning of the file without disturbing the call values. Please take a look at 'Jump To Block' for reference on the values.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct TZXCallSequenceBlockHeader
        {
            /// <summary> Number of calls to be made </summary>
            public readonly ushort callsToBeMade;
        }
        [StructLayout(LayoutKind.Sequential, Pack = 1)]

        struct TZXCallSequenceBlock
        {
            /// <summary> The block header </summary>
            public readonly TZXCallSequenceBlockHeader header;
            /// <summary> Array of call block numbers (relative-signed offsets) </summary>
            public readonly short[] callSequence;
        }


        /// <summary> ID 27 - Return from sequence
        /// This block indicates the end of the Called Sequence. The next block played will be the block after the last CALL block (or the next Call, if the Call block had multiple calls).
        ///
        ///     Again, this block has no body.
        ///
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]

        struct TZXReturnFromSequenceBlock
        {
        }

        /// <summary> ID 28 - Select block
        /// This block is useful when the tape consists of two or more separately-loadable parts. With this block, you are able to select one of the parts and the utility/emulator will start loading from that block. For example you can use it when the game has a separate Trainer or when it is a multiload. Of course, to make some use of it the emulator/utility has to show a menu with the selections when it encounters such a block. All offsets are relative signed words.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct TZXSelectBlockHeader
        {
            /// <summary> Length of the whole block (without these two bytes)  </summary>
            public readonly ushort blockLength;
            /// <summary> Number of selections </summary>
            public readonly byte nbSelections;
        }
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct TZXSelectBlock
        {
            /// <summary> Block header </summary>
            public readonly TZXSelectBlockHeader header;
            /// <summary> Number of selections </summary>
            public readonly BlockSelect[] blocks;
        }

        /// <summary> BlockSelect structure format </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct BlockSelect
        {
            /// <summary> Relative Offset </summary>
            public readonly short blockRelativeOffset;
            /// <summary> Length of description text </summary>
            public readonly byte descriptionTextLength;
            /// <summary> Description Text (please use single line and max. 30 chars) </summary>
            public readonly byte[] descriptionText;
        }


        /// <summary> ID 2A - Stop the tape if in 48K mode
        ///When this block is encountered, the tape will stop ONLY if the machine is an 48K Spectrum. This block is to be used for multiloading games that load one level at a time in 48K mode, but load the entire tape at once if in 128K mode.
        ///
        ///     This block has no body of its own, but follows the extension rule        
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct TZXStopTapeIf48KBlock
        {
            /// <summary> Length of the block without these four bytes (0) </summary>
            public readonly uint blockLength;
        }

        /// <summary> ID 2B - Set signal level
        /// This block sets the current signal level to the specified value (high or low). It should be used whenever it is necessary to avoid any ambiguities, e.g. with custom loaders which are level-sensitive.
        /// 
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct TZXSignalLevelBlock
        {
            /// <summary> Block length (without these four bytes) </summary>
            public readonly uint blockLength;
            /// <summary> Signal level (0=low, 1=high) </summary>
            public readonly byte signalLevel;
        }

        /// <summary> ID 30 - Text description
        /// This is meant to identify parts of the tape, so you know where level 1 starts, where to rewind to when the game ends, etc. This description is not guaranteed to be shown while the tape is playing, but can be read while browsing the tape or changing the tape pointer.
        ///
        ///      The description can be up to 255 characters long but please keep it down to about 30 so the programs can show it in one line(where this is appropriate).
        ///
        ///Please use 'Archive Info' block for title, authors, publisher, etc.
        ///
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct TZXTextDescriptionBlockHeader
        {
            /// <summary> Time (in seconds) for which the message should be displayed  </summary>
            public readonly byte displayMessageTimerVal;
            /// <summary> Length of the text message </summary>
            public readonly byte messageLength;
        }
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct TZXTextDescriptionBlock
        {
            /// <summary> Block header </summary>
            public readonly TZXTextDescriptionBlockHeader header;
            /// <summary>Message that should be displayed in ASCII format  </summary>
            public readonly byte[] message;
        }


        /// <summary> ID 32 - Archive Info
        /// Use this block at the beginning of the tape to identify the title of the game, author, publisher, year of publication, price (including the currency), type of software (arcade adventure, puzzle, word processor, ...), protection scheme it uses (Speedlock 1, Alkatraz, ...) and its origin (Original, Budget re-release, ...), etc. This block is built in a way that allows easy future expansion. The block consists of a series of text strings. Each text has its identification number (which tells us what the text means) and then the ASCII text. To make it possible to skip this block, if needed, the length of the whole block is at the beginning of it.
        ///
        ///        If all texts on the tape are in English language then you don't have to supply the 'Language' field
        ///
        ///The information about what hardware the tape uses is in the 'Hardware Type' block, so no need for it here.
        ///</summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct TZXArchiveInfoBlockHeader
        {
            /// <summary> Length of the whole block (without these two bytes) </summary>
            public readonly ushort blockLength;
            /// <summary> Number of TZXArchiveInfoText strings </summary>
            public readonly byte nbTextStrings;
        }
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct TZXArchiveInfoBlock
        {
            /// <summary> Block header </summary>
            public readonly TZXArchiveInfoBlockHeader header;
            /// <summary> List of TZXArchiveInfoText strings </summary>
            public readonly TZXArchiveInfoText[] textStrings;
        }
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct TZXArchiveInfoTextHeader
        {
            /// <summary> Text identification byte 
            ///     00 - Full title
            ///     01 - Software house/publisher
            ///     02 - Author(s)
            ///     03 - Year of publication
            ///     04 - Language
            ///     05 - Game/utility type
            ///     06 - Price
            ///     07 - Protection scheme/loader
            ///     08 - Origin
            ///     FF - Comment(s)
            /// </summary>
            public readonly byte type;
            /// <summary> Lenght of Text string </summary>
            public readonly byte textLength;
        }
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct TZXArchiveInfoText
        {
            public readonly TZXArchiveInfoTextHeader header;
            /// <summary> Text string in ASCII format </summary>
            public readonly byte[] text;
        }


        /// <summary> ID 33 - Hardware type 
        /// This blocks contains information about the hardware that the programs on this tape use. Please include only machines and hardware for which you are 100% sure that it either runs (or doesn't run) on or with, or you know it uses (or doesn't use) the hardware or special features of that machine.
        ///
        ///        If the tape runs only on the ZX81(and TS1000, etc.) then it clearly won't work on any Spectrum or Spectrum variant, so there's no need to list this information.
        ///
        ///If you are not sure or you haven't tested a tape on some particular machine/hardware combination then do not include it in the list.
        ///
        ///
        ///The list of hardware types and IDs is somewhat large, and may be found at the end of the format description.
        ///
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct TZXHardwareTypeBlockHeader
        {
            /// <summary> Number of machines and hardware types for which info is supplied </summary>
            public readonly byte nbHardwareInfo;
        }
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct TZXHardwareTypeBlock
        {
            /// <summary> Block header </summary>
            public readonly TZXHardwareTypeBlockHeader header;
            /// <summary> List of machines and hardware </summary>
            public readonly TZXHardwareTypeInfo[] hardwareInfo;
        }
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct TZXHardwareTypeInfo
        {
            /// <summary> Hardware type </summary>
            public readonly byte type;
            /// <summary> Hardware ID
            public readonly byte id;
            /// <summary> Hardware informations:
            ///     00 - The tape RUNS on the machine or with this hardware, but may or may not use the hardware or special features of the machine</summary>
            ///     01 - The tape USES the hardware of special features of the machine, such as extra memory of a sound chip
            ///     02 - The tape RUNS but it DOESN'T use the hardware of special features of the machine.
            ///     03 - The tape DOESN'T RUN on this machine or with this hardware
            ///     </summary>
            public readonly byte infos;
        }

        /// <summary> ID 35 - Custom info block 
        /// This block can be used to save any information you want. For example, it might contain some information written by a utility, extra settings required by a particular emulator, or even poke data.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct TZXCustomInfoBlockHeader
        {
            ///<summary>  Identification string (in ASCII)  </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x10)]
            public readonly byte[] identificationString;
            ///<summary> Lenght of the custom info </summary>
            public readonly uint customInfoLenght;
        }
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct TZXCustomInfoBlock
        {
            ///<summary> Block header </summary>
            public readonly TZXCustomInfoBlockHeader header;
            ///<summary> Custom info </summary>
            public readonly byte[] customInfo;
        }

        /// <summary> ID 5A - "Glue" block
        /// This block is generated when you merge two ZX Tape files together. It is here so that you can easily copy the files together and use them. Of course, this means that resulting file would be 10 bytes longer than if this block was not used. All you have to do if you encounter this block ID is to skip next 9 bytes.
        ///If you can avoid using this block for this purpose, then do so; it is preferable to use a utility to join the two files and ensure that they are both of the higher version number.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct TZXGlueBlock
        {
            ///<summary> Block header </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]
            public readonly byte[] ignored;
        }


        /// ===== DEPRECATED BLOCKS IDS (but can still be encountered in an old TZX) ====


        /// <summary> ID 16 - C64 ROM Type Data Block
        /// Well, this block was created to support the Commodore 64 standard ROM and similar tape blocks. It is made so basically anything that uses two or four pulses (which are the same in pairs) per bit can be written with it.

        ///        Some explanation:
        ///A wave consists of TWO pulses.The structure contains the length of ONE pulse.
        ///     The wave MUST always start with the LOW amplitude, since the C64 can only detect the transition HIGH -> LOW.
        ///     If some pulse length is 0 then the whole wave must not be present. This applies to DATA too.
        ///     The XOR checksum (if it is set to 0 or 1) is a XOR of all bits in the byte XOR-ed with the value in this field as the start value.
        ///     Finish Byte waves should be played after each byte EXCEPT last one.
        ///     Finish Data waves should be ONLY played after last byte of data.
        ///     When all the Data has finished there is an optional Trailer Tone, which is standard for the Repeated Blocks in C64 ROM Loader.
        ///The replay procedure looks like this:
        ///     Pilot Tone
        ///     Sync waves
        ///     Data Bytes (with XOR and/or Finish Byte waves)
        ///     Finish Data pulses
        ///     Trailing Tone
        ///The numbers in brackets[] represent the values for C64 ROM loader.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct TZXC64RomTypeDataBlockHeader
        {
            /// <summary> Length of the WHOLE block including the data (extension rule)</summary>
            public readonly uint wholeBlockLength;
            /// <summary> PILOT TONE pulse length [616] </summary>
            public readonly ushort pilotTonePulseLength;
            /// <summary> Number of waves in PILOT TONE </summary>
            public readonly ushort nbWavesInPilotTone;
            /// <summary> SYNC first wave pulse length [1176] </summary>
            public readonly ushort firstSyncWavePulseLength;
            /// <summary> SYNC 2nd wave pulse length [896] </summary>
            public readonly ushort secondSyncWavePulseLength;
            /// <summary> ZERO bit 1st wave pulse length [616] </summary>
            public readonly ushort firstWaveZEROBitPulseLength;
            /// <summary> ZERO bit 2nd wave pulse length [896] </summary>
            public readonly ushort secondWaveZEROBitPulseLenght;
            /// <summary>ONE bit 1st wave pulse length [896] </summary>
            public readonly ushort firstWaveONEBitPulseLength;
            /// <summary>ONE bit 2nd wave pulse length [616] </summary>
            public readonly ushort secondWaveONEBitPulseLength;
            /// <summary>
            /// XOR Checksum bit for each Data byte: [1]
            ///     00 - Start XOR checksum with value 0
            ///     01 - Start XOR checksum with value 1
            ///     FF - No checksum bit
            /// </summary>
            public readonly byte xorChecksum;
            /// <summary>FINISH BYTE 1st wave pulse length [1176] </summary>
            public readonly ushort firstWaveFINISHBytePulseLength;
            /// <summary>FINISH BYTE 2nd wave pulse length [896] </summary>
            public readonly ushort secondWaveFINISHBYTEPulseLength;
            /// <summary>FINISH DATA 1st wave pulse length [1176] </summary>
            public readonly ushort firstWaveFINISHDATAPulseLength;
            /// <summary>FINISH DATA 2nd wave pulse length [616] </summary>
            public readonly ushort secondWaveFINISHDATAPulseLength;
            /// <summary>TRAILING TONE pulse length [616] </summary>
            public readonly ushort firstWaveTRAILINGTONEPulseLength;
            /// <summary>Number of waves in TRAILING TONE </summary>
            public readonly ushort nbWavesInTrailingTone;
            /// <summary>Used bits in last byte (other bits should be 0)
            ///         (e.g. if this is 6, then the bits used(x) in last byte are: xxxxxx00)
            /// </summary>
            public readonly byte usedBitsInLastByte;
            /// <summary>General Purpose, bit-mapped: [1]
            ///         bit 0 - Data Endianess: 0=LSb first, 1=MSb first
            ///         </summary>
            public readonly byte generalPurposeFlags;
            /// <summary>Pause after this block in milliseconds (ms.) </summary>
            public readonly ushort pauseAfterThisBlock;
            /// <summary>Length of following data </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public readonly byte[] dataLength;
        }
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct TZXC64RomTypeDataBlock
        {
            ///<summary> Block header </summary>
            public readonly TZXC64RomTypeDataBlockHeader header;
            ///<summary> Data as in .TAP files </summary>
            public readonly byte[] data;
        }


        /// <summary> ID 17 - C64 Turbo Tape Data Block
        /// This block is made to support another type of encoding that is commonly used by the C64. Most of the commercial software uses this type of encoding, i.e. the Pilot tone is not made from one type of Wave only, but it is made from actual Data byte which is repeated many times. As the Sync value another, different, Data byte is sent to signal the start of the data. The Data Bits are made from ONE wave only and there is NO XOR checksum either! Trailing byte is played AFTER the DATA has ended.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct TZXC64TurboTapeDataBlockHeader
        {
            /// <summary> Length of the WHOLE block including the data (extension rule)</summary>
            public readonly uint wholeBlockLength;
            /// <summary> ZERO bit pulse </summary>
            public readonly ushort zeroBitPulse;
            /// <summary> ONE bit pulse </summary>
            public readonly ushort oneBitPulse;
            /// <summary> Additional bits in bytes (bit-mapped):
            ///     bits 0-1: number of bits (0-3)
            ///     bit 2: play additional bit(s) BEFORE (0) or AFTER (1) the byte
            ///     bit 3 : value of additional bit(s) 
            /// </summary>
            public readonly byte additionalBitsInBytes;
            /// <summary> Number of lead-in bytes</summary>
            public readonly ushort nbLeadInBytes;
            /// <summary> Lead-in byte </summary>
            public readonly byte leadInByte;
            /// <summary> Used bits in last byte (other bits should be 0) [8]
            ///     (e.g. if this is 6, then the bits used(x) in last byte are: xxxxxx00)
            /// </summary>
            public readonly byte usedBitsInLastByte;
            /// <summary> General purpose (bit-mapped) [0]
            ///            bit 0: little(0) or big(1) endian format
            ///       </summary>
            public readonly byte generalPurposeFlags;
            /// <summary> Number of trailing bytes </summary>
            public readonly short nbTrailingBytes;
            /// <summary> Trailing byte </summary>
            public readonly byte trailingByte;
            /// <summary> Pause after this block in milliseconds (ms.) </summary>
            public readonly ushort pauseAfterThisBlock;
            /// <summary> Length of following data </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public readonly byte[] dataLength;
        }
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct TZXC64TurboTapeDataBlock
        {
            ///<summary> Block header </summary>
            public readonly TZXC64TurboTapeDataBlockHeader header;
            ///<summary> Data as in .TAP files </summary>
            public readonly byte[] data;
        }


        /// <summary> ID 34 - Emulation info
        /// This is a special block that would normally be generated only by emulators. For now it contains info on everything I could find that other formats support. Please inform me of any additions/corrections since this is a very important part for emulators.
        ///        Those bits that are not used by the emulator that stored the info, should be left at their DEFAULT values. 
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct TZXEmulationInfoBlock
        {
            ///<summary> General emulation flags :
            ///     bit 0 : R-register emulation [1]
            ///     bit 1 : LDIR emulation[1]
            ///     bit 2 : high resolution colour emulation with true interrupt freq. [1]
            ///     bit 3,4 : video synchronisation : 1=high, 3=low, 0,2=normal[0]
            ///     bit 5 : fast loading when ROM load routine is used[1]
            ///     bit 6 : border emulation [1]
            ///     bit 7 : screen refresh mode (1: ON, 0: OFF) [1]
            ///     bit 8 : start playing the tape immediately[0]
            ///            If this is 0 then the emulator should only load the info blocks
            ///            and WAIT when it encounters first DATA block
            ///     bit 9 : auto type LOAD"" or press ENTER when in 128k mode[0]
            ///
            /// </summary>
            public readonly ushort generalEmulationFlags;
            ///<summary> Screen refresh delay : 1 - 255 (interrupts between refreshes) [1]
            ///(used when screen refresh mode is ON)
            ///</summary>
            public readonly byte screenRefreshDelay;
            ///<summary> Interrupt Frequency : 0 - 999 Hz  </summary>
            public readonly ushort interrupt_frequency;
            ///<summary> Reserved for future expansion </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public readonly byte[] reserved;
        }


        /// <summary> ID 40 - SnapShot Block: 
        /// This would enable one to snapshot the game at the start and still have all the tape blocks (level data, etc.) in the same file. Only .Z80 and .SNA snapshots are supported for compatibility reasons!
        /// The emulator should take care of that the snapshot is not taken while the actual Tape loading is taking place(which doesn't do much sense). And when an emulator encounters the snapshot block it should load it and then continue with the next block.
        ///  </summary>    
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct TZXSnapshotBlock
        {
            /// <summary> Snapshot type : 
            ///         00 - .Z80 format
            ///         01 - .SNA format
            /// </summary>
            public readonly byte snapshotType;
            /// <summary> Snapshot length </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public readonly byte[] snapshotLength;
            /// <summary> The snapshot file </summary>
            public readonly byte[] file;
        }





        // CUSTOM INFO BLOCKS (ID 35) common data : 

        /// <summary> ID 35 - POKEs blocks: 
        ///     See : TZXCustomInfoBlockHeader
        ///     identificationString = 	"POKEs" + 11 spaces
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct TZXCustomInfoBlockDataPokes
        {
            ///<summary>  General description length  </summary>
            public readonly byte generalDescriptionLength;
            ///<summary>  General description in ASCII format  </summary>
            public readonly byte[] generalDescription;
            ///<summary> Number of trainers </summary>
            public readonly uint nbTrainers;
            ///<summary> Trainers </summary>
            public readonly TZXCustomInfoBlockDataPokesTrainer[] trainers;

        }
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct TZXCustomInfoBlockDataPokesTrainer
        {
            ///<summary> Trainer description length  </summary>
            public readonly byte trainerDescriptionLength;
            ///<summary> Trainer Description Length </summary>
            public readonly byte[] trainerDescription;
            ///<summary> Number of pokes in this trainer </summary>
            public readonly byte nbPokes;
            ///<summary> POKEs definitions </summary>
            public readonly TZXCustomInfoBlockDataPokesPokes[] pokes;
        }
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct TZXCustomInfoBlockDataPokesPokes
        {
            ///<summary> POKE type:
            ///     bit 0-2 : memory page number
            ///     bit 3 : ignore memory page number
            ///     bit 4 : user inserts the POKE value
            ///     bit 5 : unknown original value
            /// </summary>
            public readonly byte pokeType;
            ///<summary> POKE Address </summary>
            public readonly ushort address;
            ///<summary> POKE value (leave 0 if 'user inserts' bit set) </summary>
            public readonly byte value;
            ///<summary> POKE original value (leave '0' if unknown bit set) </summary>
            public readonly byte originalValue;
        }



        /// <summary> ID 35 - Instructions block: 
        ///     See : TZXCustomInfoBlockHeader
        ///     identificationString = 	"Instructions" + 4 spaces 
        ///     </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct TZXCustomInfoBlockDataInstructions
        {
            /// <summary>
            /// Instructions text in ASCII format
            /// This block can hold any general .TXT file, with the main purpose of storing the instructions to the program or game that is in the tape.
            ///
            ///         To ensure consistency with all other ASCII texts in this format please use a single CR character(13 dec, 0D hex) to separate lines; also please use only up to 80 characters per line.
            ///
            /// </summary>
            public readonly byte[] instructionsText;
        }

        /// <summary> ID 35 - Spectrum screen block: 
        ///     See : TZXCustomInfoBlockHeader
        ///     identificationString = 	"Spectrum Screen" + 1 spaces
        ///        If the game on the tape is not an original and lacks the original loading screen then you can supply it separately within this block.This is also very handy when you want the loading screen stored separately because the original is either encrypted(like with the 'Speedlock' or 'Alkatraz' loaders) or it is corrupted by some on-screen info(like the 'Bleepload' loader). Of course not only loading screens can be stored here... you can use it to store maps or any other picture that is in Spectrum Video format(that's why the Description is there for), but because the Loading Screen will be the most common you can just set the description length field to 0 when you use it for that. Also the border colour can be specified.
        ///  </summary>    
     [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct TZXCustomInfoBlockDataSpectrumScreen
        {
            /// <summary> Description length (if this is 0 then handle it as 'Loading Screen') </summary>
            public readonly byte descriptionLength;
            /// <summary> Description of the picture in ASCII format </summary>
            public readonly byte[] pictureDescription;
            /// <summary> BORDER Colour in Spectrum colour format (0=black, 1=blue, ...) </summary>
            public readonly byte borderColour;
            /// <summary> Screen in standard Spectrum video format </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6912)]
            public readonly byte[] screenData;
        }

        /// <summary> ID 35 - ZX-Edit document block: 
        ///     See : TZXCustomInfoBlockHeader
        ///     identificationString = 	"ZX-Edit document"
        ///     This block can hold files created with the new utility called ZX-Editor. This utility gives documents the look and feel of ZX-Spectrum and its documents can contain text, graphics (with Spectrum attributes), different type faces, colours, etc. Normally these files use extension .ZED. Also the description is added, in case you want to use it for something else than 'Instructions' - you can use it for MAPs, etc.
        ///  </summary>    
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct TZXCustomInfoBlockDataZXEditDocument
        {
            /// <summary> Description length (if this is 0 then handle it as 'Instructions') </summary>
            public readonly byte descriptionLength;
            /// <summary> Description of the document in ASCII format </summary>
            public readonly byte[] documentDescription;
            /// <summary> The ZX-Editor document (.ZED file) </summary>
            public readonly byte[] zedFile;
        }

        /// <summary> ID 35 - Picture block: 
        ///     See : TZXCustomInfoBlockHeader
        ///     identificationString = 	"Picture" + 9 spaces
        ///    Finally you can include any picture(in supported formats) in the TZX file too.So cover pictures, maps, etc.can now be included in full colour (or whatever the formats supports). The best way for utilities to use this block is to spawn an external viewer, or the authors can write their own viewers (yeah, right ;) ). For inlay cards and other pictures that have zillions of colours use the JPEG format, for more simple pictures (drawing, maps, etc.) use the GIF format.
        ///  </summary>    
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct TZXCustomInfoBlockDataPicture
        {
            /// <summary> Picture format : 
            ///         00 - Gif
            ///         01 - Jpeg
            /// </summary>
            public readonly byte pictureFormat;
            /// <summary> Description length (if this is 0 then handle it as 'Inlay Card') </summary>
            public readonly byte descriptionLength;
            /// <summary> Description of the document in ASCII format </summary>
            public readonly byte[] pictureDescription;
            /// <summary> The file itself </summary>
            public readonly byte[] file;
        }

    }
    }