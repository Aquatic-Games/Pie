using System.Runtime.InteropServices;

namespace Pie.Text.Native;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct FT_List
{
    public FT_ListNode* Head;
    public FT_ListNode* Tail;
}