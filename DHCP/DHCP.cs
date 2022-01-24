using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHCP
{
  internal class DHCP
  {
    public string Mac_Address { get; set; }
    public string IP_Address { get; set; }

    public DHCP(string mac_Address, string iP_Address)
    {
      Mac_Address = mac_Address;
      IP_Address = iP_Address;
    }
  }
}
