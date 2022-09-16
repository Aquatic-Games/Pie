namespace Pie.Freetype.Native;

public enum FT_Encoding : uint
{
    None = 0,
    
    MSSymbol = ('s' << 24) | ('y' << 16) | ('m' << 8) | 'b',
    Unicode = ('u' << 24) | ('n' << 16) | ('i' << 8) | 'c',
    
    SJIS = ('s' << 24) | ('j' << 16) | ('i' << 8) | 's',
    PRC = ('g' << 24) | ('b' << 16) | (' ' << 8) | ' ',
    Big5 = ('b' << 24) | ('i' << 16) | ('g' << 8) | '5',
    Wansung = ('w' << 24) | ('a' << 16) | ('n' << 8) | 's',
    Johab = ('j' << 24) | ('o' << 16) | ('h' << 8) | 'a',
    
    GB2312 = PRC,
    MSSJIS = SJIS,
    MSGB2312 = PRC,
    MSBig5 = Big5,
    MSWansung = Wansung,
    MSJohab = Johab,
    
    AdobeStandard = ('A' << 24) | ('D' << 16) | ('O' << 8) | 'B',
    AdobeExpert = ('A' << 24) | ('D' << 16) | ('B' << 8) | 'E',
    AdobeCustom = ('A' << 24) | ('D' << 16) | ('B' << 8) | 'C',
    AdobeLatin1 = ('l' << 24) | ('a' << 16) | ('t' << 8) | '1',
    
    OldLatin2 = ('l' << 24) | ('a' << 16) | ('t' << 8) | '2',
    
    AppleRoman = ('a' << 24) | ('r' << 16) | ('m' << 8) | 'n',
}