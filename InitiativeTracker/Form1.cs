namespace InitiativeTracker
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btn_AddPokemon_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtbx_InitiativeInput.Text) || String.IsNullOrEmpty(txtbx_Pok�input.Text))
            {
                return;
            }

            bool isInitiativeValid = int.TryParse(txtbx_InitiativeInput.Text, out int initiative);

            if (!isInitiativeValid ||
                initiative < 1)
            { return; }

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
            UpdateTracker();
        }

        private void UpdateTracker()
        {
            lstvw_InitTracker.Items.Clear();
            DataHandling.ActivePokemon = (from pokemon in DataHandling.ActivePokemon
                                          orderby pokemon.Initiative descending, pokemon.Dexterity descending
                                          select pokemon).ToList();

            foreach (Pokemon pokemon in DataHandling.ActivePokemon)
            {
                ListViewItem item = new ListViewItem(pokemon.Name);
                item.SubItems.Add(pokemon.Initiative.ToString());
                item.SubItems.Add(pokemon.SuccessesNeeded.ToString());
                item.SubItems.Add(pokemon.Dexterity.ToString());

                lstvw_InitTracker.Items.Add(item);

            }
        }

        private void btn_NewRound_Click(object sender, EventArgs e)
        {
            if (DataHandling.ActivePokemon.Count == 0) return;
            DataHandling.NewRound();
            UpdateTracker();
            lbl_RoundCount.Text = DataHandling.Round.ToString();
            UpdateTurnLabel();
        }

        private void btn_UseAction_Click(object sender, EventArgs e)
        {
            DataHandling.UseAction();
            UpdateTracker();
        }

        private void btn_UpdatePokemon_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtbx_Pok�input.Text) || String.IsNullOrEmpty(txtbx_Pok�input.Text)) return;

            Pokemon? pokemonToUpdate = DataHandling.GetPokemonByName(txtbx_Pok�input.Text);
            if (pokemonToUpdate == null) return;

            bool isInitiativeValid = int.TryParse(txtbx_InitiativeInput.Text, out int initiative);
            if (!isInitiativeValid || initiative < 1) return;

            bool isDexterityValid = int.TryParse(txtbx_DexInput.Text, out int dexterity);

            if (!isDexterityValid) DataHandling.UpdatePokemon(pokemonToUpdate, initiative);
            else DataHandling.UpdatePokemon(pokemonToUpdate, initiative, dexterity);

            UpdateTracker();
        }

        private void lstvw_InitTracker_SelectedItemChanged(object sender, EventArgs e)
        {
            txtbx_Pok�input.Text = lstvw_InitTracker.SelectedItems[0].Text;
            lstvw_InitTracker.SelectedItems[0].Focused = false;
            txtbx_Pok�input.Focus();


        }

        private void btn_NextTurn_Click(object sender, EventArgs e)
        {
            bool result = DataHandling.TryNextTurn();

            if (result) UpdateTurnLabel();
        }

        private void UpdateTurnLabel()
        {
            lbl_Turn.Text = $"It's {DataHandling.CurrentPokemon.Name}'s Turn!";
        }

        private void btn_FaintClick(object sender, EventArgs e)
        {
            bool result = DataHandling.TryFaintPokemon();
            if (!result) return;

            UpdateTracker();
            UpdateTurnLabel();
        }

        private void btn_Reset_Click(object sender, EventArgs e)
        {
            DataHandling.Reset();
            UpdateTracker();
            lbl_RoundCount.Text = "0";
            lbl_Turn.Text = "Who's Turn Is It?";

        }
    }
}
