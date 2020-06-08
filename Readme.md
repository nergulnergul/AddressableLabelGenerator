 * Author : Jeewon Park 
 * Description : Generate static variables of Addressable Labels to manage it EASY.
 * How to use : 
 ```
 1. just put the script in /Assets/Editor/ 
 2. make Labels in Addressable
 3. assign labels to your addressable assets
 4. Hit Deckard Utils / Get All Labels in menu bar
 5. then you can see the AutoGenLabels.cs in Scripts/AllLabels folder
 6. it is private static variables, 
    6-1. however, you can access in anywhere with AutoGenLabels.GetLabels() to get all labels 
    6-2. return type is List<String>
 7. It should be handy, if you make a downloader.
 ```
 * Worked in : 
 ```
 1. Unity 2019.3.0f
 2. Addressable 1.5
 ```
