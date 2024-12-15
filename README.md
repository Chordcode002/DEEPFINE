# Unity 런타임 OBJ 로더 구현

> 기능 설명
1. 시작하면 3개의 버튼으로 구성된 UI를 볼 수 있습니다.
    * Load OBJ : 단일 OBJ 파일을 실행창에서 선택하여 로드합니다.(1번 구현)
    * Load Multiple OBJ : 지정된 경로(/Resources/Models)의 모든 OBJ를 로드합니다. (3번 구현)
    * Clear OBJ : 로드한 모든 OBJ 오브젝트를 제거합니다.
2. 1번>2번>3번으로 발전시켜 나가는 것으로 이해하고, 1번 기능과 3번 기능을 동시에 사용할 수 있도록 장면을 구성하였습니다. 2번은 LoadAssetAsync를 단일로 사용하는 것, 3번은 동시에 여러번 사용하는 것으로 이해했기때문에 2번은 따로 구현하지 않았습니다. 만약 폴더 내의 OBJ가 하나라면, 2번으로 동작할 것 입니다.
3. 단일 OBJ 로드는 0.5초 이내에 대부분 처리되며, 20개의 OBJ 파일의 경우 .obj 파일만 75MB, .mtl과 텍스쳐 파일까지 총 118MB가 4초 정도안에 처리됩니다.

> 로드 과정 설명 (3번, Load Multiple OBJ)
1. AssetLoader가 지정된 경로의 폴더 내의 모든 obj파일을 탐색합니다.
2. 모든 obj파일의 파일 경로를 filePaths에 담습니다.
3. 크기 순으로 오름차순 정렬합니다.
4. 정렬한 filePaths를 매개로 Load를 실행합니다.
5. filePaths의 수만큼 LoaderModule의 LoadAssetAsync를 비동기적으로 수행합니다.
6. LoadAssetAsync는 OBJLoader를 통해 obj파일을 읽고, 파싱하고, 게임오브젝트를 생성하여 메쉬, 머테리얼, 쉐이더를 할당합니다.
7. OBJLoader의 작업은 유니티 API를 사용하기 때문에, 외부 쓰레드에서 실행될 수 없습니다. 따라서 UnityMainThread를 통해 메인쓰레드에 올려 수행합니다. 
8. 이 과정은 비동기성을 해치지만, 결국 유니티 API에 접근을 하는 과정이 있으면 무조건 거기서 오버헤드가 제일 많이 발생할 것입니다. 메쉬, 텍스쳐, 머테리얼을 미리 파싱하더라도, 결국 할당하는 과정은 유니티 API에서 하기때문에 그렇습니다. 결국 중간에 메인쓰레드에 접근하냐, 마지막에 한꺼번에 접근하냐의 문제라고 생각하여 이렇게 구현하였습니다.
9. 로드가 완료되면, 보기 좋게 하기 위해 4*5 격자모양으로 정해진 간격으로 재배치합니다.
10. 각 LoadAssetAsync 동작이 완료될 때마다, 프로그레스 바에 콜백을 통해 완료됨을 전달합니다.

> 참고
* OBJ로더로는 Dummiesman의 OBJ Loader를 사용하였습니다. 직접 OBJ 로드 하는 코드를 구현해서 하려 했으나 현재 OBJLoader는 머테리얼과 텍스쳐를 불러와주고 안정적이어서 실제 프로젝트 소스로 구현시 퀄리티를 생각한다면 OBJLoader를 사용하는 것이 더 효과적이라 사용하였습니다.
    * [링크] <https://assetstore.unity.com/packages/tools/modeling/runtime-obj-importer-49547?locale=ko-KR&srsltid=AfmBOop_0DCiI1QxjHrCmfi70VT0S2DTznWXXlq8G0hOezr6pz5oJcFk>
* 비동기적으로 메인 쓰레드에 작업을 올리는 특성상 간혹 메인 쓰레드에 작업을 올리지 못하는 문제가 발생할 수 있습니다. 재실행해주시면 됩니다.
