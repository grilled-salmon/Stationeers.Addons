﻿using Assets.Scripts;
using Assets.Scripts.Steam;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;

namespace Stationeers.Addons.Modules.Workshop
{
    public class WorkshopManagerPatch
    {
        // Why does System.IO not have a directory copy method?
        // Stolen from here: https://stackoverflow.com/questions/1974019/folder-copy-in-c-sharp
        private static void copyDirectory(string strSource, string strDestination)
        {
            if (!Directory.Exists(strDestination))
            {
                Directory.CreateDirectory(strDestination);
            }

            DirectoryInfo dirInfo = new DirectoryInfo(strSource);
            FileInfo[] files = dirInfo.GetFiles();
            foreach (FileInfo tempfile in files)
            {
                tempfile.CopyTo(Path.Combine(strDestination, tempfile.Name));
            }

            DirectoryInfo[] directories = dirInfo.GetDirectories();
            foreach (DirectoryInfo tempdir in directories)
            {
                copyDirectory(Path.Combine(strSource, tempdir.Name), Path.Combine(strDestination, tempdir.Name));
            }

        }

        public static void PublishWorkshopPrefix(WorkshopManager __instance, ref WorkShopItemDetail ItemDetail, ref string changeNote, out string __state)
        {
            string origItemContentPath = ItemDetail.Path;
            string tempItemContentPath = origItemContentPath + "_temp";
            __state = "";

            DirectoryInfo tempItemContentDir = Directory.CreateDirectory(tempItemContentPath);

            // Copy files to upload to temp directory, if they satisfy upload whitelist.
            foreach (string itemFilePath in Directory.GetFiles(origItemContentPath))
            {
                bool validFile = false;
                string fileName = new FileInfo(itemFilePath).Name;

                // Ugly fall-through regex party.
                if (new Regex(@".*\.cs$").IsMatch(fileName))
                    validFile = true;

                if (new Regex(@".*\.xml$").IsMatch(fileName))
                    validFile = true;

                if (new Regex(@".*\.png$").IsMatch(fileName))
                    validFile = true;

                if (new Regex(@".*\.asset$").IsMatch(fileName))
                    validFile = true;

                if (new Regex(@"^LICENSE$").IsMatch(fileName))
                    validFile = true;

                if (validFile)
                    File.Copy(itemFilePath, tempItemContentPath + Path.GetFileName(itemFilePath));
            }

            // Copy directories to upload to temp directory, if they satisfy upload whitelist.
            foreach (string itemFolderPath in Directory.GetDirectories(origItemContentPath))
            {
                bool validDir = false;
                string dirName = new DirectoryInfo(itemFolderPath).Name;

                // Ugly fall-through regex party.
                if (new Regex(@"^About$").IsMatch(dirName))
                    validDir = true;

                if (new Regex(@"^Content$").IsMatch(dirName))
                    validDir = true;

                if (new Regex(@"^GameData$").IsMatch(dirName))
                    validDir = true;

                if (new Regex(@"^Scripts$").IsMatch(dirName))
                    validDir = true;

                if (validDir)
                    copyDirectory(itemFolderPath, tempItemContentPath + Path.DirectorySeparatorChar + dirName);
            }

            // Set workshop item info to use temporary path before ISteamUCG gets its hands on it.
            ItemDetail.Path = tempItemContentPath;

            // Save state for later nuking.
            Debug.Log("Created temporary workshop item directory " + __state);
            __state = tempItemContentPath;
        }

        public static void OnCreateItemPostfix(WorkshopManager __instance, SteamAsyncCreateItem Parent, bool WasSuccessful, WorkShopItemDetail ItemDetail, bool UserNeedsToAcceptWorkshopLegalAgreement)
        {
            string origItemContentPath = ItemDetail.Path;
            string tempItemContentPath = origItemContentPath + "_temp";

            if (Directory.Exists(tempItemContentPath))
            {
                // Recursively remove the temp dir after steam is done with it.
                Debug.Log("Cleared temporary workshop item directory " + tempItemContentPath);
                Directory.Delete(tempItemContentPath, true);
            }
        }
    }
}
