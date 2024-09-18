using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using UnityEngine;

public static class NetworkUtilities
{
    public static string GetLocalIPv4(string preferredSubnet = "192.168.")
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                if (ip.ToString().StartsWith(preferredSubnet))
                {
                    return ip.ToString();
                }
            }
        }

        return host.AddressList
            .FirstOrDefault(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            ?.ToString();
    }

    public static string GetSubnetMask(string ipAddress)
    {
        foreach (var networkInterface in NetworkInterface.GetAllNetworkInterfaces())
        {
            foreach (var unicastIPAddressInformation in networkInterface.GetIPProperties().UnicastAddresses)
            {
                if (unicastIPAddressInformation.Address.ToString() == ipAddress)
                {
                    return unicastIPAddressInformation.IPv4Mask.ToString();
                }
            }
        }
        throw new Exception("Subnet Mask Not Found!");
    }

    public static List<string> GetIPRange(string ipAddress, string subnetMask)
    {
        // ChatGPT Code
        List<string> ipRange = new List<string>();

        byte[] ipBytes = IPAddress.Parse(ipAddress).GetAddressBytes();
        byte[] maskBytes = IPAddress.Parse(subnetMask).GetAddressBytes();

        byte[] startIP = new byte[4];
        byte[] endIP = new byte[4];

        for (int i = 0; i < 4; i++)
        {
            startIP[i] = (byte)(ipBytes[i] & maskBytes[i]);
            endIP[i] = (byte)(ipBytes[i] | ~maskBytes[i]);
        }

        for (uint i = BitConverter.ToUInt32(startIP.Reverse().ToArray(), 0); i <= BitConverter.ToUInt32(endIP.Reverse().ToArray(), 0); i++)
            ipRange.Add(new IPAddress(BitConverter.GetBytes(i).Reverse().ToArray()).ToString());        

        return ipRange;
    }

    public static List<string> GetIPRange(string ipAddress)
    {
        // Hard Coded Method if I can say so
        List<string> ipRange = new List<string>();

        string[] bytes = ipAddress.Split('.');

        string prefix = bytes[0] + '.' + bytes[1] + '.' + bytes[2] + '.';

        // We know that the max byte value is 255
        for (int i = 0; i <= 255; i++)
            ipRange.Add(prefix + i);

        return ipRange;
    }
}
