using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DHCP
{
  internal class Program
  {
    static Dictionary<string,string> dhcp_adatok = new Dictionary<string, string>();
    static List<string> excluded = new List<string>();
    static Dictionary<string,string> reserved = new Dictionary<string, string>();
    static Program program = new Program();
    static int elso_Kioszthato_Cim = 100;
    static void request(string MAC_Address)
    {
      //Mac címnek van érvényes foglalása
      if (dhcp_adatok.ContainsKey(MAC_Address))
      {
        return;
      }
      //Mac címnek nincs érvényes foglalása
      else
      {
        //Mac cím szerepel a fenntartásokban
        if (dhcp_adatok.ContainsKey(MAC_Address))
        {
          string IP = "";
          foreach (var item in reserved)
          {
            if (item.Key.Equals(MAC_Address))
            {
              IP = item.Value;
            }
          }
          //IP ki van már osztva?
          if (dhcp_adatok.ContainsValue(IP))
          {
            return;
          }
          // IP cím nincs kiosztva
          else
          {
            dhcp_adatok.Add(MAC_Address,IP);
          }
        }
        //Mac cím nem szerepel a fenntartásokban
        else
        {
          string IP = $"192.168.{elso_Kioszthato_Cim}";

          //IP cím ki van már osztva?
          if (dhcp_adatok.ContainsValue(IP))
          {
            while (dhcp_adatok.ContainsValue(IP) && elso_Kioszthato_Cim<200)
            {
              elso_Kioszthato_Cim++;
              IP = $"192.168.{elso_Kioszthato_Cim}";
            }
            if (elso_Kioszthato_Cim<200)
            {
              dhcp_adatok.Add(MAC_Address,IP);
            }
            else
            {
              Console.WriteLine("Nem sikerült kiosztani!");
              return;
            }
          }
          //IP cím nincs kiosztva
          else
          {
            //Nincs a kizártak között
            if (!excluded.Contains(IP))
            {
              //IP cím a fenntartások között van
              if (!reserved.ContainsValue(IP))
              {
                dhcp_adatok.Add(MAC_Address, IP);
              }
              else
              {
                while (dhcp_adatok.ContainsValue(IP) && elso_Kioszthato_Cim < 200)
                {
                  elso_Kioszthato_Cim++;
                  IP = $"192.168.{elso_Kioszthato_Cim}";
                }
                if (elso_Kioszthato_Cim < 200)
                {
                  dhcp_adatok.Add(MAC_Address, IP);
                }
                else
                {
                  Console.WriteLine("Nem sikerült kiosztani!");
                  return;
                }
              }
            }
            //IP cím a kizártak között van
            else
            {
              while (dhcp_adatok.ContainsValue(IP) && elso_Kioszthato_Cim < 200)
              {
                elso_Kioszthato_Cim++;
                IP = $"192.168.{elso_Kioszthato_Cim}";
              }
              if (elso_Kioszthato_Cim < 200)
              {
                dhcp_adatok.Add(MAC_Address, IP);
              }
              else
              {
                Console.WriteLine("Nem sikerült kiosztani!");
                return;
              }
            }
          }
        }
      }
    }
    static void release(string MAC_Address)
    {
      foreach (var item in dhcp_adatok)
      {
        if (item.Key.Equals(MAC_Address))
        {
          dhcp_adatok.Remove(item.Key);
        }
      }
    }

    static void Main(string[] args)
    {
      Beolvasas();
      KiirasaFajlba();
      Console.ReadKey();
    }

    private static void KiirasaFajlba()
    {
      using (StreamWriter ki = new StreamWriter("dhcp_kesz.csv"))
      {
        foreach (var item in dhcp_adatok)
        {
          ki.WriteLine($"{item.Key} - {item.Value}");
        }
      }
    }

    private static void Beolvasas()
    {
      using (StreamReader be = new StreamReader("dhcp.csv"))
      {
        while (!be.EndOfStream)
        {
          string[] a = be.ReadLine().Split(';');
          dhcp_adatok.Add(a[0], a[1]);
        }
      }
      using (StreamReader be = new StreamReader("excluded.csv"))
      {
        while (!be.EndOfStream)
        {
          excluded.Add(be.ReadLine());
        }
      }
      using (StreamReader be = new StreamReader("reserved.csv"))
      {
        while (!be.EndOfStream)
        {
          string[] a = be.ReadLine().Split(';');
          reserved.Add(a[0], a[1]);
        }
      }
      using (StreamReader be = new StreamReader("test.csv"))
      {
        while (!be.EndOfStream)
        {
          string[] a = be.ReadLine().Split(';');
          if (a[0] == "request")
          {
            request(a[1]);
          }
          else
          {
            release(a[1]);
          }
        }
      }
    }
    static bool Sikertelen_Kiosztas_Ellenorzes()
    {
      if (elso_Kioszthato_Cim > 199)
      {
        return true;
      }
      return false;
    }
  }
}
