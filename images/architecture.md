```mermaid
graph TD
    subgraph Presentation
        Views[Views]
        ViewModels[ViewModels]
    end
    
    subgraph Business
        Services[Services]
        AIService[AIService]
    end
    
    subgraph Data
        Models[Models]
        Configuration[Configuration]
    end
    
    subgraph Utilities
        Helpers[Helpers]
        Security[Security]
        Logging[Logging]
    end
    
    ViewModels --> Services
    ViewModels --> AIService
    Views --> ViewModels
    Services --> Models
    Services --> Helpers
    Services --> AIService
    AIService --> Configuration
    AIService --> Logging
    Services --> Configuration
    Helpers --> Security
    Helpers --> Logging
    Security --> Configuration
```
