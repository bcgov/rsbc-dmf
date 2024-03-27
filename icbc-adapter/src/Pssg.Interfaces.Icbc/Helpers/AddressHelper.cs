using Pssg.Interfaces.Icbc.Models;
using System;

public static class AddressHelper
{
    public static string ToAddressLine1(this CLNT data) 
    {
        if (data?.ADDR != null)
        {
            var addressComponents = "";

            // Unit Number
            if (!string.IsNullOrEmpty(data.ADDR.BUNO))
            {
                addressComponents += $"{data.ADDR.BUNO}-";
            }

            // Street Number

            if (!string.IsNullOrEmpty(data.ADDR.STNO))
            {
                addressComponents += $"{data.ADDR.STNO}";
            }

            // AddressPrefix1
            if (!string.IsNullOrEmpty(data.ADDR.APR1))
            {
                addressComponents += $"{(addressComponents.Length > 0 ? ' ' : String.Empty)}{data.ADDR.APR1}";
            }

            //AddressPrefix2
            if (!string.IsNullOrEmpty(data.ADDR.APR2))
            {
                addressComponents += $" {data.ADDR.APR2}";
            }

            //AddressPrefix3 // Not in ICBC Client
            if (!string.IsNullOrEmpty(data.ADDR.APR3))
            {
                addressComponents += $" {data.ADDR.APR3}";
            }

            // ???

            if (!string.IsNullOrEmpty(data.ADDR.PSTN))
            {
                addressComponents += $"STN {data.ADDR.PSTN}";
            }

            // Site
            if (!string.IsNullOrEmpty(data.ADDR.SITE))
            {
                addressComponents += $"SITE {data.ADDR.SITE}";
            }

            // Compound

            if (!string.IsNullOrEmpty(data.ADDR.COMP))
            {
                addressComponents += $"COMP {data.ADDR.COMP}";
            }

            // RuralRoute
            if (!string.IsNullOrEmpty(data.ADDR.RURR))
            {
                addressComponents += $"RR# {data.ADDR.RURR}";
            }

            //Street Name
            if (!string.IsNullOrEmpty(data.ADDR.STNM))
            {
                addressComponents += $" {data.ADDR.STNM}";
            }

            // Street Type
            if (!string.IsNullOrEmpty(data.ADDR.STTY))
            {
                addressComponents += $" {data.ADDR.STTY}";
            }

            // Street Direction
            if (!string.IsNullOrEmpty(data.ADDR.STDI))
            {
                addressComponents += $" {data.ADDR.STDI}";
            }

            // PostOfficeBox

            if (!string.IsNullOrEmpty(data.ADDR.POBX))
            {
                addressComponents += $"\n</br> PO BOX {data.ADDR.POBX}";
            }


            return addressComponents;
        }

        return null;
    }
}
