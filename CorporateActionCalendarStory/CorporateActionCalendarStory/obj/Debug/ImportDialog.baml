   M S B A M L   `   `   `  ����W  SCorporateActionCalendarStory, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullK*clr-namespace:CorporateActionCalendarStoryCorporateActionCalendarStory  5   	   /    )CorporateActionCalendarStory.ImportDialog   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   <local*clr-namespace:CorporateActionCalendarStory     5   	     � Title$  Constituent Importer��5   	   $	��300��6&   $	��300��63   .��5	      � .+�5
   
   �� -   5         ��Name$ ImportTitle��5
      $
��Left=�6)   $��23��6D   $��	10,26,0,0q�6P   $
��Wrap=�6c   $2��,Would you like to import a parent and child?��6w   $	��Top=�6�   $	��272��6�   �� 5   
   -   5        $ parentLabel��5      $
��Left=�6)   $��	34,81,0,0q�6D   $
��Wrap=�6W   $��Parent Ticker��6k   $	��Top=�6�    J� IsHitTestVisible .  6�   �� 5   
   -   5         ��Name$ txtParentTicker��5      $
��Left=�6+   $��23��6F   $��
34,102,0,0q�6R    �� TextWrapping$
 Wrap=�6f   $�� ��6z   $	��Top=�6�   $	��110��6�   �� 5   
   -   5        $ ChildTickerLabel��5      $
��Left=�6.   $��
149,81,0,0q�6I   $
��Wrap=�6]   $��Child Ticker��6q   $	��Top=�6�    .  6�   �� 5   
   -   5        $ txtChildTicker��5      $
��Left=�6*   $��23��6E   $��149,102,0,0q�6Q   $
 Wrap=�6f   $�� ��6z   $	��Top=�6�   $	��110��6�   �� 5   
   -   5         ��Name$ btnImportParent��5      $��Import��6*   $
��Left=�6;   $��
69,169,0,0q�6V   $	��Top=�6j   $��75��6�   6�   �� 5   
   -   5        $ 	btnCancel��5      $��Cancel��6$   $
��Left=�65   $��149,169,0,0q�6P   $	��Top=�6e   $��75��6}   6�   5      5      