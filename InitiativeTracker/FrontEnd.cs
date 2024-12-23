namespace InitiativeTracker
{
    public partial class FrontEnd : Form
    {
        public FrontEnd()
        {
            InitializeComponent();
        }

        private void btn_AddPokemon_Click(object sender, EventArgs e)
        {
            ColorsReset();

            bool areFieldsValid = ValidateInputFields();
            if (!areFieldsValid) return;   

            bool isInitiativeValid = int.TryParse(txtbx_InitiativeInput.Text, out int initiative);

            if (!isInitiativeValid || initiative < 1)
            {
                txtbx_InitiativeInput.BackColor = Color.Salmon;
                return; 
            }

            bool isDexterityValid = int.TryParse(txtbx_DexInput.Text, out int dexterity);

            if (!isDexterityValid)
            {
                Pokemon newPokemon = new(txtbx_Pok�input.Text, initiative);
                DataHandling.ActivePokemon.Add(newPokemon);
            }
            else
            {
                Pokemon newPokemon = new(txtbx_Pok�input.Text, initiative, dexterity);
                DataHandling.ActivePokemon.Add(newPokemon);
            }
            UpdateTracker(true);


            txtbx_DexInput.Text = "";
            txtbx_InitiativeInput.Text = "";
        }

        private bool ValidateInputFields()
        {
            if (String.IsNullOrEmpty(txtbx_InitiativeInput.Text) && String.IsNullOrEmpty(txtbx_Pok�input.Text))
            {
                txtbx_InitiativeInput.BackColor = Color.Salmon;
                txtbx_Pok�input.BackColor = Color.Salmon;
                return false;
            }

            if (String.IsNullOrEmpty(txtbx_InitiativeInput.Text))
            {
                txtbx_InitiativeInput.BackColor = Color.Salmon;
                return false;
            }

            if (String.IsNullOrEmpty(txtbx_Pok�input.Text))
            {
                txtbx_Pok�input.BackColor = Color.Salmon;
                return false;
            }

            return true;
        }

        private void ColorsReset()
        {
            txtbx_InitiativeInput.BackColor = SystemColors.Window;
            txtbx_Pok�input.BackColor = SystemColors.Window;
            txtbx_DexInput.BackColor = SystemColors.Window;
        }

        private void UpdateTracker(bool resort)
        {
            bool trickRoom = GetTrickRoomStatus();

            lstvw_InitTracker.Items.Clear();
            if (resort && !trickRoom)
            {
                DataHandling.ActivePokemon = (from pokemon in DataHandling.ActivePokemon
                                              orderby pokemon.Initiative descending, pokemon.Dexterity descending
                                              select pokemon).ToList();
            }

            if (resort && trickRoom)
            {
                DataHandling.ActivePokemon = (from pokemon in DataHandling.ActivePokemon
                                              orderby pokemon.Initiative, pokemon.Dexterity
                                              select pokemon).ToList();
            }

            foreach (Pokemon pokemon in DataHandling.ActivePokemon)
            {
                ListViewItem item = new ListViewItem(pokemon.Name);
                item.SubItems.Add(pokemon.Initiative.ToString());
                item.SubItems.Add(pokemon.SuccessesNeeded.ToString());
                item.SubItems.Add(pokemon.Dexterity.ToString());

                lstvw_InitTracker.Items.Add(item);

            }

            if (DataHandling.CurrentPokemon != null) HighlightCurrentMon();
        }

        private bool GetTrickRoomStatus()
        {
            if (chkbx_TrickRoom.Checked) return true;
            else return false;
        }
        private void btn_NewRound_Click(object sender, EventArgs e)
        {
            if (DataHandling.ActivePokemon.Count == 0) return;

            DataHandling.NewRound();
            UpdateTracker(true);
            if (DataHandling.Round > 0) btn_NewRound.Text = "New Round";
            lbl_RoundCount.Text = DataHandling.Round.ToString();
            txtbx_Pok�input.Text = DataHandling.CurrentPokemon.Name;
            UpdateTurnLabel();
            HighlightCurrentMon();
        }

        private void btn_UseAction_Click(object sender, EventArgs e)
        {
            Pokemon? selectedMon = DataHandling.GetPokemonByName(txtbx_Pok�input.Text);
            if (selectedMon == null) return;
            DataHandling.UseAction(selectedMon);
            UpdateTracker(false);
        }

        private void btn_UpdatePokemon_Click(object sender, EventArgs e)
        {
            ColorsReset();

            bool areFieldsValid = ValidateInputFields();
            if (!areFieldsValid) return;

            Pokemon? pokemonToUpdate = DataHandling.GetPokemonByName(txtbx_Pok�input.Text);
            if (pokemonToUpdate == null) 
            {
                txtbx_Pok�input.BackColor = Color.Salmon;
                return; 
            }

            bool isInitiativeValid = int.TryParse(txtbx_InitiativeInput.Text, out int initiative);
            if (!isInitiativeValid || initiative < 1)
            { 
                txtbx_InitiativeInput.BackColor = Color.Salmon;
                return; 
            }

            bool isDexterityValid = int.TryParse(txtbx_DexInput.Text, out int dexterity);

            if (!isDexterityValid && String.IsNullOrEmpty(txtbx_DexInput.Text))
            {
                DataHandling.UpdatePokemon(pokemonToUpdate, initiative);
            }
            else if (!isDexterityValid)
            {
                txtbx_DexInput.BackColor = Color.Salmon;
                DataHandling.UpdatePokemon(pokemonToUpdate, initiative);
            }
            else DataHandling.UpdatePokemon(pokemonToUpdate, initiative, dexterity);

            UpdateTracker(true);
        }

        private void CheckTrickRoomChanged(object sender, EventArgs e)
        {
            DataHandling.ActivePokemon.Reverse();
            UpdateTracker(false);
        }

        private void lstvw_InitTracker_SelectedItemChanged(object sender, EventArgs e)
        {
            txtbx_Pok�input.Text = lstvw_InitTracker.SelectedItems[0].Text;
            lstvw_InitTracker.SelectedItems[0].Focused = false;
            txtbx_Pok�input.Focus();


        }

        private void btn_NextTurn_Click(object sender, EventArgs e)
        {
            if (DataHandling.Round == 0) return;

            bool result = DataHandling.TryNextTurn();

            if (result)
            {
                UpdateTurnLabel();
                HighlightCurrentMon();
            }

            txtbx_Pok�input.Text = DataHandling.CurrentPokemon.Name;
        }

        private void UpdateTurnLabel()
        {
            lbl_Turn.Text = $"It's {DataHandling.CurrentPokemon.Name}'s Turn!";
        }
        private void HighlightCurrentMon()
        {
            foreach (ListViewItem pokemon in lstvw_InitTracker.Items)
            {
                if (pokemon.Text == DataHandling.CurrentPokemon.Name)
                {
                    pokemon.BackColor = Color.LightGreen;
                }
                else pokemon.BackColor = Color.Gray;
            }
        }

        private void btn_FaintClick(object sender, EventArgs e)
        {
            Pokemon? faintedMon = DataHandling.GetPokemonByName(txtbx_Pok�input.Text);
            if (faintedMon == null) return;

            DataHandling.TryFaintPokemon(faintedMon);
            UpdateTracker(false);
            UpdateTurnLabel();
        }

        private void btn_Reset_Click(object sender, EventArgs e)
        {
            DataHandling.Reset();
            UpdateTracker(false);
            lbl_RoundCount.Text = "0";
            lbl_Turn.Text = "Who's Turn Is It?";
            btn_NewRound.Text = "Start!";

        }

        private void btn_MoveUp_Click(object sender, EventArgs e)
        {
            Pokemon? movingMon = DataHandling.GetPokemonByName(txtbx_Pok�input.Text);
            if (movingMon == null) return;

            DataHandling.MoveUp(movingMon);
            UpdateTracker(false);
        }

        private void btn_MoveDown_Click(object sender, EventArgs e)
        {
            Pokemon? movingMon = DataHandling.GetPokemonByName(txtbx_Pok�input.Text);
            if (movingMon == null) return;

            DataHandling.MoveDown(movingMon);
            UpdateTracker(false);
        }

        private void btn_Help_Click(object sender, EventArgs e)
        {
            string message = """
                            The "Add Pokemon" button adds a new Pok�mon to the initative. Pokemon must have a name and Initiative, but dex is optional.
                            When you are ready to start the fight, press "Start!" to begin!

                            You can update pokemon Initative and Dex at the end of the round by double clicking them, putting in the new numbers, and hitting Update Pokemon.
                            Then click New Round to start the next round!

                            You can see how many Successes any given Pok�mon needs to succeed in the "SN" column of the tracker.

                            You can move any pokemon up or down, update their details, faint them, or increase their needed successes by double clicking their name in the tracker and hitting the relevant button.

                            You can check the "Trick Room" box to reverse the turn order.
                            """;
            MessageBox.Show(message, "Help");
        }
    }
}
