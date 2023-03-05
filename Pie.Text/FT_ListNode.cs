using System.Runtime.InteropServices;

namespace Pie.Text;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct FT_ListNode
{
    public FT_ListNode* Prev;
    public FT_ListNode* Next;
    public void* Data;
}