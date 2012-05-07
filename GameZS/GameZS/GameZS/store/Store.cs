using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework;
using System.IO;
using ZombieSmashers.quake;

namespace ZombieSmashers.store
{
    public class Store
    {
        public StorageDevice device;
        IAsyncResult deviceResult;

        public StorageContainer container;
        public bool pendingDevice;
        bool containerOpen;

        public const int STORE_SETTINGS = 0;
        public const int STORE_GAME = 1;

        private string[] storeStr = { 
            "settings.sav", 
            "game.sav" 
        };

        

       

        public void GetDevice()
        {
            //deviceResult = Guide.BeginShowStorageDeviceSelector(PlayerIndex.One, null, null);
            if (!Guide.IsVisible)
            {
                deviceResult = StorageDevice.BeginShowSelector(PlayerIndex.One, null, null);
            }
            pendingDevice = true;
        }

        public void Update()
        {
            if (pendingDevice)
            {
                if (deviceResult.IsCompleted)
                {
                    //device = Guide.EndShowStorageDeviceSelector(deviceResult);
                    device = StorageDevice.EndShowSelector(deviceResult);
                    pendingDevice = false;
                    Read(STORE_SETTINGS);
                }
            }
        }

        private bool CheckDeviceFail()
        {
            if (pendingDevice)
                return true;
            if (device == null)
                return true;
            if (!device.IsConnected)
                return true;
            return false;
        }

        private void OpenContainer()
        {
            if (!containerOpen)
                //container = device.OpenContainer("ZombieSmashers");
                container = OpenContainer(device,"ZombieSmashers");

            containerOpen = true;
        }

        private static StorageContainer OpenContainer(StorageDevice storageDevice, string saveGameName)
        {
            IAsyncResult result = storageDevice.BeginOpenContainer(saveGameName, null, null);

            // Wait for the WaitHandle to become signaled.
            result.AsyncWaitHandle.WaitOne();

            StorageContainer container = storageDevice.EndOpenContainer(result);

            // Close the wait handle.
            result.AsyncWaitHandle.Close();

            return container;
        }

        public void Write(int type)
        {
            if (CheckDeviceFail())
                return;

            OpenContainer();

            //string fileName = Path.Combine(container.Path, storeStr[STORE_SETTINGS]);
            string fileName = storeStr[STORE_SETTINGS];

            FileStream file = File.Open(fileName, FileMode.OpenOrCreate, FileAccess.Write);

            BinaryWriter writer = new BinaryWriter(file);

            switch (type)
            {
                case STORE_SETTINGS:
                    Game1.settings.Write(writer);
                    break;
            }

            file.Close();

        }

        public void Read(int type)
        {
            if (CheckDeviceFail())
                return;

            OpenContainer();

            //string fileName = Path.Combine(container.Path, storeStr[STORE_SETTINGS]);
            string fileName = storeStr[STORE_SETTINGS];

            FileStream file;
            if (!File.Exists(fileName))
            {
                return;
            }
            else
                file = File.Open(fileName, FileMode.Open, FileAccess.Read);

            BinaryReader reader = new BinaryReader(file);

            switch (type)
            {
                case STORE_SETTINGS:
                    Game1.settings.Read(reader);
                    break;
            }

            file.Close();

        }
    }
}
