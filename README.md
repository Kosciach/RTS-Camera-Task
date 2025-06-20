# Kamera RTS

[![YouTube](https://img.shields.io/badge/YouTube-red?style=for-the-badge&logo=youtube&logoColor=white)](https://www.youtube.com/watch?v=dQw4w9WgXcQ)

Projekt ma na celu stworzenie kamery według podanych wytycznych na potrzeby zadania rekrutacyjnego.
Moim celem było aby cały system był elastyczny, solidny ale i zarazem prosty.

---
<br>

## Features:
  ### Pan & Drag
  - Ruch za pomocą WSAD
  - Przeciąganie myszy z wciśniętym LMB
  - Parametry: `Prędkość` `Przyśpieszenie` `Inercja` 

  ### Edge Scrolling
  - Przesuwanie kamery za pomocą krawędzi ekranu
  - Parametry: `Procentowy zakres odległości od ekrau` 
    
  ### Zoom
  - Płynny zoom in/out za pomocą scroll
  - Parametry: `Ograniczenie zakresu` `Prędkość` `Wygładzenie`

  ### Obracanie
  - Przeciąganie myszy z wciśniętym RMB
  - Parametry: `Pionowy zakres obrotu` `Prędkość` `Przyśpieszenie` `Inercja`  

  ### Granice ruchu
  - Utrzymanie kamery w określonym obszarze
  - Proste narzędzie do ustalenia obszaru poruszani się kamery

## Wykonanie:
  ### Assety
  - `Unity 6`
  - `Cinemachine` - Główny element sterujący kamerą
  - `Input System` - Obsługa inputu
  - `Naughty Attributes` - Usprawnienia inspektora

  ### Architektura
  Głównym komponentem kamery rts, nie licząc cinemachine jest `RTSCameraController` - jego zadaniem jest kontrolowanie komponentów odpowiadających za zachowanie kamery.<br>
  Aby osiągnąc modularność komponenty kamery dziedziczą z `RTSCameraComponent` - ten zawiera podstawowe metody jak `Setup, Tick, Dispose` oraz zmienne `Config, InputMgr`.
  Aktualne komponenty kamery:
  - `RTSCameraMove` - odpowiada za poruszanie kamery.
  - `RTSCameraRotate` - odpowiada za obrut kamery.
  - `RTSCameraZoom` - odpowiada za przybliżenie/oddalenie kamery.
     
  Dzięki takiemu podejściu bardzo łatwo jest zmienić to jak kamera się zachowuje, poprzez usunięcie/wymienienie jednego z komponentów.
  Dodatkowo config jest ScriptableObjectem, co dalej zwiększa modularność, pozwalając na zmiane configu z np mocno wygładzonego, powolnego, na szybki, agresywny.

<br>

  Za ustalenie obszaru poruszania kamery odpowiada `RTSCameraBounds`, używa on listy Vector2 które tworzą wielokąt.
  Metoda `ClampPos` zwraca pozycje ograniczoną tak aby zawsze znajdowała się w tym wielokącie.
  Aby ułatwić ustawienie obszaru, dodałem edytor wyświetlający na scenie line oraz kule które można przesuwać.

<br>

  Input pobierany jest z prostego `InputManager`, którego zadaniem jest tworzenie i aktywacja inputów wymaganych przez kamerę.
  Został zaprojektowany tak, aby w przyszłości można go było łatwo rozbudować lub całkowicie wymienić.

## Potencjalne ulepszenie:
  - `Indywidualne configi` - każdy komponent mógłby posiadać własny config, co umożliwiłoby jeszcze większą elastyczność i lepsze dostosowanie parametrów.
  - `Lepsze zarządzanie inputem` - obecnie wykorzystywany InputManager można w przyszłości rozbudować o obsługę różnych źródeł sterowania (np. pod pad), dzięki temu system stanie się bardziej uniwersalny.
